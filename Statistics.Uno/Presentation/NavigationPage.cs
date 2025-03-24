using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.UI.Xaml.Controls;

namespace Statistics.Uno.Presentation;

public sealed partial class NavigationPage : Page
{
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
            var contentGrid = new Grid();

            const int columnOneWidth = 10;
            const int columnTwoWidth = 100 - columnOneWidth;

            contentGrid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            contentGrid.ColumnDefinitions(new GridLength(columnOneWidth, GridUnitType.Star),
                new GridLength(columnTwoWidth, GridUnitType.Star));
            contentGrid.VerticalAlignment = VerticalAlignment.Stretch;
            contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            var navigationListView = CreateNavigationListView().Grid(row: 0, column: 0);
            var navigationFrame = CreateNavigationFrame().Grid(row: 0, column: 1);

            contentGrid.Children.Add(navigationListView);
            contentGrid.Children.Add(navigationFrame);

            return contentGrid;
        }

        private Frame CreateNavigationFrame()
        {
            var frame = new Frame();

            logic.RegisterNavigationFrame(frame);

            return frame;
        }

        private ListView CreateNavigationListView()
        {
            var view = new ListView();

            var options = new List<TextBlock>()
            {
                CreateResponseNavigationTextBlock(view),
                CreatePromptsNavigationTextBlock(view),
                CreateArtificialIntelligenceTextBlock(view),
            };

            view.SelectionChanged += logic.ListViewOnSelectionChanged;
            view.ItemsSource = options;

            return view;
        }

        private TextBlock CreateResponseNavigationTextBlock(ListView view)
        {
            var item = CreateBaseNavigationTextBlock(view);

            item.Text = "Responses";

            return item;
        }

        private TextBlock CreateBaseNavigationTextBlock(ListView view)
        {
            return new TextBlock()
            {
                Padding = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = view.Background,
                FocusVisualPrimaryBrush = view.Background,
                FocusVisualSecondaryBrush = view.Background,
            };
        }

        private TextBlock CreatePromptsNavigationTextBlock(ListView view)
        {
            var item = CreateBaseNavigationTextBlock(view);

            item.Text = "Register Prompts";

            return item;
        }

        private TextBlock CreateArtificialIntelligenceTextBlock(ListView view)
        {
            var item = CreateBaseNavigationTextBlock(view);

            item.Text = "Register Artificial Intelligences ";

            return item;
        }
    }

    private class NavigationPageLogic
    {
        private Frame? navigationFrame;

        internal void RegisterNavigationFrame(Frame frame)
        {
            navigationFrame ??= frame;
        }

        internal void ListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (navigationFrame == null)
            {
                throw new NullReferenceException(
                    $"Expected '{nameof(navigationFrame)}' reference to be set, but it was not.");
            }

            // Temporary Navigation to test with.
            navigationFrame.Navigate(typeof(MainPage));
        }
    }
}
