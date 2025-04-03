using Statistics.Shared.Extensions;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class NavigationPage : Page
{
    private enum Pages
    {
        RESPONSES = 0,
        PROMPTS = 1,
        ARTIFICIAL_INTELLIGENCES = 2,
        KEYWORDS = 3,
        APPLIED_KEYWORDS = 4
    }

    public NavigationPage()
    {
        DataContext = new NavigationViewModel();

        var logic = new NavigationPageLogic();
        var ui = new NavigationPageUi(logic, (NavigationViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());

        logic.UpdateNavigationFrame();
    }

    private class NavigationPageUi : BaseUi<NavigationPageLogic, NavigationViewModel>
    {
        private readonly NavigationPageLogic logic;

        public NavigationPageUi(NavigationPageLogic logic, NavigationViewModel dataContext) : base(logic, dataContext)
        {
            this.logic = logic;
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
        {
            const int columnOneWidth = 12;
            const int columnTwoWidth = 100 - columnOneWidth;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.ColumnDefinitions(new GridLength(columnOneWidth, GridUnitType.Star),
                new GridLength(columnTwoWidth, GridUnitType.Star));

            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            ListView navigationListView = CreateNavigationListView().Grid(row: 0, column: 0);
            Frame navigationFrame = CreateNavigationFrame().Grid(row: 0, column: 1);

            grid.Children.Add(navigationListView);
            grid.Children.Add(navigationFrame);
        }

        private Frame CreateNavigationFrame()
        {
            var frame = new Frame() {Background = new SolidColorBrush(Colors.White),};

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
                        new Setter(BackgroundProperty, new SolidColorBrush(Colors.Black)),
                        new Setter(ForegroundProperty, new SolidColorBrush(Colors.White)),
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

            Page newPage = selectedPage switch
            {
                Pages.RESPONSES => new ResponsesPage(),
                Pages.PROMPTS => new PromptsPage(),
                Pages.ARTIFICIAL_INTELLIGENCES => new ArtificialIntelligencePage(),
                Pages.KEYWORDS => new KeywordsPage(),
                Pages.APPLIED_KEYWORDS => new AppliedKeywordsPage(),
                _ => new DefaultPage()
            };

            Console.WriteLine($"Navigation to '{newPage.GetType().Name}'...");
            navigationFrame!.Navigate(newPage.GetType());
        }
    }
}
