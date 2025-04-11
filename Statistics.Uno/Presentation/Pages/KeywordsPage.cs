using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class KeywordsPage : BasePage
{
    public KeywordsPage()
    {
        IKeywordEndpoint keywordApi = GetKeywordApi();

        DataContext = new KeywordsViewModel();

        var logic = new KeywordsPageLogic(keywordApi, (KeywordsViewModel) DataContext, this);
        var ui = new KeywordsPageUi(logic, (KeywordsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateDisplayedItems();
    }

    private enum DataGridColumns
    {
        KEYWORD_TEXT = 0,
        USE_REGULAR_EXPRESSION = 1,
        START_SEARCH = 2,
        END_SEARCH = 3,
        CREATED_AT = 4,
        LAST_UPDATED_AT = 5,
        ACTIONS = 6,
    }
}
