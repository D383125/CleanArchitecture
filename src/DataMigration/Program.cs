using DbUp;

internal class Program
{
    private static void Main(string[] args)
    {
        //TODO: Read from Secrets manager
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Error: Connection string not found.");
            return;
        }

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem("SqlScripts") 
                .LogToConsole()
                .Build();

        
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
            return;
#endif            
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Database upgrade successful!");
        Console.ResetColor();
      
    }
}