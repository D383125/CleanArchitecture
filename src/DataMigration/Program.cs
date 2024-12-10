using DbUp;

internal class Program
{
    private static void Main(string[] args)
    {
        var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=SupportBot";        
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem("SqlScripts") // Folder containing your SQL scripts
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