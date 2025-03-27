using Statistics.Shared.Extensions;

namespace Statistics.Uno.Presentation;

public sealed partial class NavigationPage : Page
{
    private enum Pages
    {
        RESPONSES = 0,
        PROMPTS = 1,
        ARTIFICIAL_INTELLIGENCES = 2,
    }

    public NavigationPage()
    {
        var logic = new NavigationPageLogic();
        var ui = new NavigationPageUi(logic);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class NavigationPageUi
    {
        private readonly NavigationPageLogic logic;

        public NavigationPageUi(NavigationPageLogic logic)
        {
            this.logic = logic;
        }

        internal Grid CreateContentGrid()
        {
            var grid = new Grid();

            const int columnOneWidth = 12;
            const int columnTwoWidth = 100 - columnOneWidth;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.ColumnDefinitions(new GridLength(columnOneWidth, GridUnitType.Star),
                new GridLength(columnTwoWidth, GridUnitType.Star));
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;

            ListView navigationListView = CreateNavigationListView().Grid(row: 0, column: 0);
            Frame navigationFrame = CreateNavigationFrame().Grid(row: 0, column: 1);

            grid.Children.Add(navigationListView);
            grid.Children.Add(navigationFrame);

            logic.UpdateNavigationFrame();

            return grid;
        }

        private Frame CreateNavigationFrame()
        {
            var frame = new Frame();

            logic.RegisterNavigationFrame(frame);

            return frame;
        }

        private ListView CreateNavigationListView()
        {
            var view = new ListView()
            {
                Background = new SolidColorBrush(Colors.Black),
                ItemContainerStyle = new Style(typeof(ListViewItem))
                {
                    Setters =
                    {
                        new Setter(FrameworkElement.BackgroundProperty, new SolidColorBrush(Colors.Black)),
                        new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)),
                    },
                },
            };

            var options = Enum.GetValues<Pages>().Select(x => CreateNavigationTextBlock(view, x));

            view.ItemsSource = options;
            view.SelectedIndex = (int) Pages.RESPONSES;
            view.SelectionChanged += logic.ListViewOnSelectionChanged;

            return view;
        }

        private TextBlock CreateNavigationTextBlock(ListView view, Pages page)
        {
            return new TextBlock()
            {
                Padding = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = view.Background,
                FocusVisualPrimaryBrush = view.Background,
                FocusVisualSecondaryBrush = view.Background,
                Margin = new Thickness(10),
                Foreground = new SolidColorBrush(Colors.White),
                Text = page.ToString().ScreamingSnakeCaseToTitleCase(),
            };
        }
    }

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

            switch (selectedPage)
            {
                case Pages.RESPONSES:
                    Console.WriteLine($"Navigation to '{nameof(ResponsesPage)}'...");
                    navigationFrame!.Navigate(typeof(ResponsesPage));
                    break;
                case Pages.PROMPTS:
                    Console.WriteLine($"Navigation to '{nameof(PromptsPage)}'...");
                    navigationFrame!.Navigate(typeof(PromptsPage));
                    break;
                case Pages.ARTIFICIAL_INTELLIGENCES:
                default:
                    Console.WriteLine($"Navigation to '{nameof(DefaultPage)}'...");
                    navigationFrame!.Navigate(typeof(DefaultPage));
                    break;
            }
        }
    }
}
