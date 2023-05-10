using Microsoft.AspNetCore.Builder;

namespace Server;

/// <summary>
/// The runner for the web services
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Create the web application
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        var server = new ChatServer();
        server.Configure(app);

        // Run the web application
        app.Run();
    }
}