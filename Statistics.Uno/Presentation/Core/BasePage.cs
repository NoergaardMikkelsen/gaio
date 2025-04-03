using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Uno.Endpoints;

namespace Statistics.Uno.Presentation.Core;

public abstract class BasePage : Page
{
    protected BasePage()
    {
        App = (App) Application.Current;
    }

    private App App { get; }

    protected IPromptEndpoint GetPromptApi()
    {
        return App.Startup.ServiceProvider.GetService<IPromptEndpoint>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IPromptEndpoint)}'.");
    }

    protected IArtificialIntelligenceEndpoint GetAiApi()
    {
        return App.Startup.ServiceProvider.GetService<IArtificialIntelligenceEndpoint>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IArtificialIntelligenceEndpoint)}'.");
    }

    protected IResponseEndpoint GetResponseApi()
    {
        return App.Startup.ServiceProvider.GetService<IResponseEndpoint>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IResponseEndpoint)}'.");
    }

    protected IKeywordEndpoint GetKeywordApi()
    {
        return App.Startup.ServiceProvider.GetService<IKeywordEndpoint>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IKeywordEndpoint)}'.");
    }

    protected IActionEndpoint GetActionApi()
    {
        return App.Startup.ServiceProvider.GetService<IActionEndpoint>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IActionEndpoint)}'.");
    }

    protected IAppliedKeywordService GetAppliedKeywordService()
    {
        return App.Startup.ServiceProvider.GetService<IAppliedKeywordService>() ??
               throw new NullReferenceException(
                   $"Failed to acquire an instance implementing '{nameof(IAppliedKeywordService)}'.");
    }


}
