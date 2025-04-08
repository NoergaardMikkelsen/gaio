using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class PromptsPage : BasePage
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        CREATED_AT = 1,
        LAST_UPDATED_AT = 2,
        ACTIONS = 3,
    }

    public PromptsPage()
    {
        IPromptEndpoint promptApi = GetPromptApi();
        IActionEndpoint actionApi = GetActionApi();

        DataContext = new PromptsViewModel();

        var logic = new PromptsPageLogic(promptApi, actionApi, (PromptsViewModel) DataContext, this);
        var ui = new PromptsPageUi(logic, (PromptsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdatePrompts();
    }
}
