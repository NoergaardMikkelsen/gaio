using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class ArtificialIntelligencePage : BasePage
{
    private enum DataGridColumns
    {
        NAME = 0,
        KEY = 1,
        AI_TYPE = 2,
        CREATED_AT = 3,
        LAST_UPDATED_AT = 4,
        ACTIONS = 5,
    }

    public ArtificialIntelligencePage()
    {
        IArtificialIntelligenceEndpoint aiApi = GetAiApi();

        DataContext = new ArtificialIntelligenceViewModel();

        var logic = new ArtificialIntelligencePageLogic(aiApi, (ArtificialIntelligenceViewModel) DataContext, this);
        var ui = new ArtificialIntelligencePageUi(logic, (ArtificialIntelligenceViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        _ = logic.UpdateDisplayedItems();
    }
}
