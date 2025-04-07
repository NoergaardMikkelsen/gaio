using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Uno.Presentation.Core;

public abstract class BaseDialogUi<TLogic, TViewModel, TEntity, TSearchable> : BaseUi<TLogic, TViewModel>
    where TLogic : BaseDialogLogic<TViewModel, TEntity, TSearchable>
    where TViewModel : class
    where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    protected readonly ContentDialog dialog;

    protected BaseDialogUi(TLogic logic, TViewModel viewModel, ContentDialog dialog) : base(logic, viewModel)
    {
        this.dialog = dialog;
        this.dialog.PrimaryButtonClick += Logic.PrimaryButtonClicked;
    }

    protected void AddLabelAndTextBox(
        Grid grid, string labelText, string bindingPath, int row, bool isEnabled = true,
        IValueConverter? converter = null, string placeholderText = "")
    {
        var label = new TextBlock()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10, 5),
        };
        label.Grid(row: row, column: 0);

        var textBox = new TextBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10, 5),
            Width = 500,
            IsEnabled = isEnabled,
            PlaceholderText = placeholderText,
            TextWrapping = TextWrapping.WrapWholeWords,
        };
        textBox.SetBinding(TextBox.TextProperty, new Binding
        {
            Path = new PropertyPath(bindingPath),
            Mode = BindingMode.TwoWay,
            Converter = converter,
        });
        textBox.Grid(row: row, column: 1);

        grid.Children.Add(label);
        grid.Children.Add(textBox);
    }


    protected void AddLabelAndComboBox(
        Grid grid, string labelText, string bindingPath, int row, IEnumerable<string> itemsSource,
        IValueConverter converter)
    {
        var label = new TextBlock()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10, 5),
        };
        label.Grid(row: row, column: 0);

        var comboBox = new ComboBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10, 5),
            Width = 200,
            ItemsSource = itemsSource,
        };
        comboBox.SetBinding(Selector.SelectedIndexProperty, new Binding
        {
            Path = new PropertyPath(bindingPath),
            Mode = BindingMode.TwoWay,
            Converter = converter,
        });
        comboBox.Grid(row: row, column: 1);

        grid.Children.Add(label);
        grid.Children.Add(comboBox);
    }

    protected void AddLabelAndCheckBox(Grid grid, string labelText, string bindingPath, int row)
    {
        var label = new TextBlock()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10, 5),
        };
        label.Grid(row: row, column: 0);

        var checkBox = new CheckBox()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10, 5),
        };
        checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding
        {
            Path = new PropertyPath(bindingPath),
            Mode = BindingMode.TwoWay,
        });
        checkBox.Grid(row: row, column: 1);

        grid.Children.Add(label);
        grid.Children.Add(checkBox);
    }
}
