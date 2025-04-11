using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class NavigationPage : Page
{
    public NavigationPage()
    {
        DataContext = new NavigationViewModel();

        var logic = new NavigationPageLogic();
        var ui = new NavigationPageUi(logic, (NavigationViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdateNavigationFrame();
    }

    private enum Pages
    {
        RESPONSES = 0,
        PROMPTS = 1,
        ARTIFICIAL_INTELLIGENCES = 2,
        KEYWORDS = 3,
        APPLIED_KEYWORDS = 4,
    }
}
