using Domain;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("Database"))
        );
            //services.AddDbContextCheck<ApplicationDbContext>();
            //builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));
            //services.Configure<ApplicationOptions>((c) => configuration.GetSection("Application"));            

            return services;
        }
    }
}
