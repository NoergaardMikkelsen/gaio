using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Refit;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ResponsesPage : BasePage
{
    private enum DataGridColumns
    {
        PROMPT_TEXT = 0,
        RESPONSE_TEXT = 1,
        CREATED_AT = 2,
    }

    public ResponsesPage()
    {
        IResponseEndpoint responseApi = GetResponseApi();
        IActionEndpoint actionApi = GetActionApi();

        DataContext = new ResponsesViewModel();

        var logic = new ResponsesPageLogic(responseApi, actionApi, (ResponsesViewModel) DataContext);
        var ui = new ResponsesPageUi(logic, (ResponsesViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateResponses();
    }
}
