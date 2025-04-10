using CommunityToolkit.WinUI.UI.Controls;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class NavigationPage
{
    private class NavigationPageLogic
    {
        private Frame? navigationFrame;
        private Pages selectedPage;

        internal void RegisterNavigationFrame(Frame frame)
        {
            navigationFrame ??= frame;
        }

        internal void ListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView) sender;

            selectedPage = (Pages) listView.SelectedIndex;
            UpdateNavigationFrame();
        }

        private void AssertNavigationFrameIsSet()
        {
            if (navigationFrame == null)
            {
                throw new NullReferenceException(
                    $"Expected '{nameof(navigationFrame)}' reference to be set, but it was not.");
            }
        }

        internal void UpdateNavigationFrame()
        {
            AssertNavigationFrameIsSet();

            Page newPage = selectedPage switch
            {
                Pages.RESPONSES => new ResponsesPage(),
                Pages.PROMPTS => new PromptsPage(),
                Pages.ARTIFICIAL_INTELLIGENCES => new ArtificialIntelligencePage(),
                Pages.KEYWORDS => new KeywordsPage(),
                Pages.APPLIED_KEYWORDS => new AppliedKeywordsPage(),
                var _ => new DefaultPage(),
            };

            Console.WriteLine($"Navigation to '{newPage.GetType().Name}'...");
            navigationFrame!.Navigate(newPage.GetType());
        }
    }
}
