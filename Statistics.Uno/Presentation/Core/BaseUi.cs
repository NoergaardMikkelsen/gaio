namespace Statistics.Uno.Presentation.Core;

public abstract class BaseUi<TLogic, TViewModel> where TLogic : class where TViewModel : class
{
    protected TLogic Logic { get; }
    protected TViewModel ViewModel { get; }

    protected BaseUi(TLogic logic, TViewModel viewModel)
    {
        Logic = logic;
        ViewModel = viewModel;
    }

    public Grid CreateContentGrid()
    {
        var grid = new Grid();

        ConfigureGrid(grid);
        AddControlsToGrid(grid);

        return grid;
    }

    protected abstract void ConfigureGrid(Grid grid);

    protected abstract void AddControlsToGrid(Grid grid);

    protected virtual StackPanel CreateRefreshButtonsPanel(Func<Task>? refreshAction = null)
    {
        if (refreshAction == null)
        {
            throw new ArgumentNullException(nameof(refreshAction));
        }

        var stackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10),
        };

        var button = new Button()
        {
            Content = "Refresh",
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        button.Click += async (_, _) => await refreshAction();

        stackPanel.Children.Add(button);

        return stackPanel;
    }
}
