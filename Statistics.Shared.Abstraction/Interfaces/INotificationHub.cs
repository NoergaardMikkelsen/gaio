using Statistics.Shared.Abstraction.Enum;

namespace Statistics.Shared.Abstraction.Interfaces;

public interface INotificationHub
{
    public Task SendEntityChangedNotification(SignalrEvent eventType, string message);
}
