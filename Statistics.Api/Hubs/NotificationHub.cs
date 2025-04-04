using Microsoft.AspNetCore.SignalR;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces;

namespace Statistics.Api.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    private readonly ILogger<NotificationHub> logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation($"New User ({Context.ConnectionId}) connected to {nameof(NotificationHub)}");

        //await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"User ({Context.ConnectionId}) disconnected from {nameof(NotificationHub)}");

        //await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    //public async Task SendPromptsUpdatedNotification(string message)
    //{
    //    logger.LogInformation($"Sending Prompts Changed notification to all clients");
    //    await Clients.All.SendEntityChangedNotification(SignalrEvent.PROMPTS_CHANGED, message);
    //}

    //public async Task SendResponsesUpdatedNotification(string message)
    //{
    //    logger.LogInformation($"Sending Responses Changed notification to all clients");
    //    await Clients.All.SendEntityChangedNotification(SignalrEvent.RESPONSES_CHANGED, message);
    //}

    //public async Task SendKeywordsUpdatedNotification(string message)
    //{
    //    logger.LogInformation($"Sending Keywords Changed notification to all clients");
    //    await Clients.All.SendEntityChangedNotification(SignalrEvent.KEYWORDS_CHANGED, message);
    //}

    //public async Task SendArtificialIntelligencesUpdatedNotification(string message)
    //{
    //    logger.LogInformation($"Sending Artificial Intelligences Changed notification to all clients");
    //    await Clients.All.SendEntityChangedNotification(SignalrEvent.ARTIFICIAL_INTELLIGENCES_CHANGED, message);
    //}
}
