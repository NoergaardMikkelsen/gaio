using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Statistics.Shared.Abstraction.Interfaces.Refit;

namespace Statistics.Commandline;

internal class Program
{
    private CommandlineStartup Startup { get; set; }

    private static void Main(string[] args)
    {
        var program = new Program();

        program.Run(args).GetAwaiter().GetResult();
        Thread.Sleep(TimeSpan.FromMinutes(1));
    }

    private async Task Run(string[] args)
    {
        Startup = new CommandlineStartup();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        Startup.SetupApplication(builder);
        Startup.SetupServices(builder.Services);

        using IHost host = builder.Build();
        Console.WriteLine("Host has been build...");

        var actionEndpoint = host.Services.GetRequiredService<IActionEndpoint>();

        var response = await actionEndpoint.ExecuteAllPrompts(CancellationToken.None);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "Successfully executed all prompts."
            : "Failed to execute all prompts.");

        await host.RunAsync();
    }
}
