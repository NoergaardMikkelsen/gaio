using Microsoft.Extensions.Hosting;
using Statistics.Shared.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Statistics.Commandline;

internal class Program
{
    private static void Main(string[] args)
    {
        var program = new Program();

        program.Run(args).GetAwaiter().GetResult();
        Console.ReadKey();
    }

    private async Task Run(string[] args)
    {
        var startup = new CommandlineStartup();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        startup.SetupApplication(builder);
        startup.SetupServices(builder.Services);

        using IHost host = builder.Build();
        Console.WriteLine("Host has been build...");

        await host.RunAsync();
    }
}
