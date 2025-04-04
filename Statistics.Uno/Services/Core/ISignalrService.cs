namespace Statistics.Uno.Services.Core;

public interface ISignalrService
{
    event EventHandler<string>? ArtificialIntelligenceChanged;
    event EventHandler<string>? KeywordsChanged;
    event EventHandler<string>? PromptsChanged;
    event EventHandler<string>? ResponsesChanged;
}
