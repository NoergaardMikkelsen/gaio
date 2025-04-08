using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class AppliedKeywordsPage : BasePage
{
    private enum DataGridColumns
    {
        TEXT = 0,
        USES_REGEX = 1,
        MATCHING_RESPONSES_COUNT = 2,
        TOTAL_RESPONSES_COUNT = 3,
        START_SEARCH = 4,
        END_SEARCH = 5,
    }

    public AppliedKeywordsPage()
    {
        IAppliedKeywordService keywordService = GetAppliedKeywordService();
        IResponseEndpoint responsesApi = GetResponseApi();
        IKeywordEndpoint keywordApi = GetKeywordApi();

        DataContext = new AppliedKeywordsViewModel();

        var logic = new AppliedKeywordsPageLogic(keywordService, responsesApi, keywordApi,
            (AppliedKeywordsViewModel) DataContext);
        var ui = new AppliedKeywordsPageUi(logic, (AppliedKeywordsViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateAppliedKeywords();
    }
}
