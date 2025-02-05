using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;
using Testcontainers.PostgreSql;

namespace Application.UnitTests
{
    public class TestContext : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres;
        public ApplicationDbContext DbContext { get; private set; }

        public TestContext()
        {
            _postgres = new PostgreSqlBuilder()
               .WithImage("postgres:15-alpine")
               .Build();
        }

        public T CreateService<T>(params object[] args) where T : class
        {
            var m = new Mock<T>(args);

            return m.Object;
        }

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            System.Diagnostics.Debug.WriteLine($"Running container {_postgres.Name} in a '{_postgres.State.ToString()}' state.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;

            DbContext = new ApplicationDbContext(options);
            await DbContext.Database.EnsureCreatedAsync();
            await WriteBasicMigrationsAsync();
        }

        public Task DisposeAsync()
        {
            DbContext?.Dispose();
            return _postgres.DisposeAsync().AsTask();
        }

        protected async Task WriteBasicMigrationsAsync()
        {
            Assembly dependencyAssembly = Assembly.Load("DataMigration");
            var script = dependencyAssembly.GetManifestResourceStream("DataMigration.SqlScripts.0001-CreateChatHistoryTable.sql");

            if (script != null)
            {
                using StreamReader sr = new(script);
                var contents = sr.ReadToEnd();

                await _postgres.ExecScriptAsync(contents);
            }
        }
    }
}
