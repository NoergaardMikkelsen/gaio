
namespace Statistics.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new ApiStartup(builder.Configuration);

        startup.SetupServices(builder.Services);
        var app = builder.Build();
        startup.SetupApplication(app);

        app.Run();
    }
}
