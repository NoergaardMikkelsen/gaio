namespace Statistics.Api;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
        var startup = new ApiStartup();

        startup.SetupServices(builder.Services);
        WebApplication? app = builder.Build();
        startup.SetupApplication(app);

        app.Run();

        // Ensure the console closes after the application stops
        Environment.Exit(0);
    }
}
