using Microsoft.AspNetCore.SignalR.Client;
using Statistics.Uno.Services;
using Statistics.Uno.Services.Core;

namespace Statistics.Uno.Startup;

public class UnoSignalrStartupModule : IUnoStartupModule
{
    private readonly HubConnection hubConnection;

    public delegate HubConnection BuildHubConnection();

    public UnoSignalrStartupModule(BuildHubConnection buildHubConnection)
    {
        Console.WriteLine($"Constructing UnoSignalrStartupModule Class...");
        hubConnection = buildHubConnection();
        Console.WriteLine($"HubConnection created");
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        Console.WriteLine($"Configuring UnoSignalrStartupModule Class...");
        var signalrService = new SignalrService(hubConnection);

        services.AddSingleton<ISignalrService>(signalrService);

        // Start the SignalR connection asynchronously
        StartSignalRConnectionAsync().ConfigureAwait(false);
    }

    private async Task StartSignalRConnectionAsync()
    {
        try
        {
            Console.WriteLine($"Attempting to establish a connection to SignarlR");
            await hubConnection.StartAsync();
            Console.WriteLine("SignalR Connection started");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting SignalR connection: {ex.GetBaseException()}");
        }
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
    }
}
