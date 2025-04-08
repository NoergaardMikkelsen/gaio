using Statistics.Shared.Extensions;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Pages.ViewModel;

namespace Statistics.Uno.Presentation.Pages;

public sealed partial class NavigationPage
{
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
}
