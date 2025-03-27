using CommunityToolkit.WinUI.UI.Controls;

public abstract class BasePageUi<TLogic, TViewModel> where TLogic : class where TViewModel : class
{
    protected TLogic Logic { get; }
    protected TViewModel DataContext { get; }

    protected BasePageUi(TLogic logic, TViewModel dataContext)
    {
        Logic = logic;
        DataContext = dataContext;
    }

    public Grid CreateContentGrid()
    {
        var grid = new Grid();

        ConfigureGridRowsAndColumns(grid);
        AddControlsToGrid(grid);

        return grid;
    }


    protected abstract void ConfigureGridRowsAndColumns(Grid grid);

    protected abstract void AddControlsToGrid(Grid grid);
}
