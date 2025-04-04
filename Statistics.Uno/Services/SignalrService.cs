using Microsoft.AspNetCore.SignalR.Client;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces;
using Statistics.Uno.Services.Core;

namespace Statistics.Uno.Services;

public class SignalrService : ISignalrService
{
    /// <inheritdoc />
    public event EventHandler<string>? ArtificialIntelligenceChanged;

    /// <inheritdoc />
    public event EventHandler<string>? KeywordsChanged;

    /// <inheritdoc />
    public event EventHandler<string>? PromptsChanged;

    /// <inheritdoc />
    public event EventHandler<string>? ResponsesChanged;

    private readonly HubConnection hubConnection;

    public SignalrService(HubConnection hubConnection)
    {
        this.hubConnection = hubConnection;

        SetupOnEvents();
    }

    private void SetupOnEvents()
    {
        hubConnection.On<SignalrEvent, string>(nameof(INotificationHub.SendEntityChangedNotification),
            SendEntityChangedNotification);
    }

    private void SendEntityChangedNotification(SignalrEvent eventType, string message)
    {
        Action<string> subMethod = eventType switch
        {
            SignalrEvent.ARTIFICIAL_INTELLIGENCES_CHANGED => TriggerArtificialIntelligenceChangedEvent,
            SignalrEvent.KEYWORDS_CHANGED => TriggerKeywordsChangedEvent,
            SignalrEvent.PROMPTS_CHANGED => TriggerPromptsChangedEvent,
            SignalrEvent.RESPONSES_CHANGED => TriggerResponsesChangedEvent,
            var _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
        };

        subMethod?.Invoke(message);
    }

    private void TriggerArtificialIntelligenceChangedEvent(string message)
    {
        ArtificialIntelligenceChanged?.Invoke(this, message);
        Console.WriteLine($"{SignalrEvent.ARTIFICIAL_INTELLIGENCES_CHANGED.ToString()} - Received message: {message}");
    }

    private void TriggerKeywordsChangedEvent(string message)
    {
        KeywordsChanged?.Invoke(this, message);
        Console.WriteLine($"{SignalrEvent.KEYWORDS_CHANGED.ToString()} - Received message: {message}");
    }

    private void TriggerPromptsChangedEvent(string message)
    {
        PromptsChanged?.Invoke(this, message);
        Console.WriteLine($"{SignalrEvent.PROMPTS_CHANGED.ToString()}  - Received message: {message}");
    }

    private void TriggerResponsesChangedEvent(string message)
    {
        ResponsesChanged?.Invoke(this, message);
        Console.WriteLine($"{SignalrEvent.RESPONSES_CHANGED.ToString()}  - Received message: {message}");
    }
}
