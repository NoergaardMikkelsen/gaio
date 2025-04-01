using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Extensions;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.Factory;

public class SetupColumnsArguments
{
    public SetupColumnsArguments(
        DataGrid dataGrid, IEnumerable<int> enumNumbers, Func<int, string> getBindingPath,
        Func<int, bool> isTemplateColumn, Func<int, string> getEnumAsString, Func<int, int> getColumnStarWidth,
        Func<int, FrameworkElement>? buildActionsElement = null)
    {
        DataGrid = dataGrid;
        EnumNumbers = enumNumbers;
        GetBindingPath = getBindingPath;
        IsTemplateColumn = isTemplateColumn;
        GetEnumAsString = getEnumAsString;
        GetColumnStarWidth = getColumnStarWidth;
        BuildActionsElement = buildActionsElement;
    }

    public DataGrid DataGrid { get; }
    public IEnumerable<int> EnumNumbers { get; }
    public Func<int, string> GetBindingPath { get; }
    public Func<int, bool> IsTemplateColumn { get; }
    public Func<int, string> GetEnumAsString { get; }
    public Func<int, int> GetColumnStarWidth { get; }
    public Func<int, FrameworkElement>? BuildActionsElement { get; }
}

public class SetupRowArguments
{
    public SetupRowArguments(
        DataGrid dataGrid, IEnumerable<int> enumNumbers, Func<int, bool> shouldApplyFormatter,
        Func<int, string> getBindingPath, Func<int, FrameworkElement>? buildActionsElement = null)
    {
        DataGrid = dataGrid;
        EnumNumbers = enumNumbers;
        ShouldApplyFormatter = shouldApplyFormatter;
        GetBindingPath = getBindingPath;
        BuildActionsElement = buildActionsElement;
    }

    public DataGrid DataGrid { get; }
    public IEnumerable<int> EnumNumbers { get; }
    public Func<int, bool> ShouldApplyFormatter { get; }
    public Func<int, string> GetBindingPath { get; }
    public Func<int, FrameworkElement>? BuildActionsElement { get; }
}

public static class DataGridFactory
{
    public static DataGrid CreateDataGrid<TViewModel>(
        TViewModel dataContext, string itemsSourcePath, Action<DataGrid> setupColumns,
        Action<DataGrid> setupRowTemplate)
    {
        var dataGrid = new DataGrid()
        {
            CanUserReorderColumns = false,
            CanUserResizeColumns = true,
            CanUserSortColumns = true,
            SelectionMode = DataGridSelectionMode.Single,
            ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
            AutoGenerateColumns = false,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = true,
        };

        setupColumns(dataGrid);
        setupRowTemplate(dataGrid);

        dataGrid.SetBinding(DataGrid.ItemsSourceProperty,
            new Binding() {Path = itemsSourcePath, Source = dataContext,});

        return dataGrid;
    }

    private static DataGridTemplateColumn CreateDataGridDateTemplateColumn(string bindingPath, string header)
    {
        var textBlock = new TextBlock()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0),
        };

        var binding = new Binding
        {
            Path = bindingPath,
            Converter = new UtcDateTimeToLocalStringConverter(),
        };

        textBlock.SetBinding(TextBlock.TextProperty, binding);

        return new DataGridTemplateColumn()
        {
            Header = header,
            CellTemplate = new DataTemplate(() => textBlock),
        };
    }

    private static DataGridTextColumn CreateDataGridTextColumn(string bindingPath, string header)
    {
        return new DataGridTextColumn()
        {
            Header = header,
            Binding = new Binding
            {
                Path = bindingPath,
            },
        };
    }

    public static void SetupDataGridRowTemplate(SetupRowArguments setupRowArguments)
    {
        var stack = new StackPanel();
        IEnumerable<FrameworkElement> cells = setupRowArguments.EnumNumbers.Select(x =>
        {
            var block = new TextBlock() {Margin = new Thickness(10),};
            string bindingPath = setupRowArguments.GetBindingPath(x);

            switch (bindingPath)
            {
                case nameof(ISearchable.Id) when setupRowArguments.BuildActionsElement != null:
                    return setupRowArguments.BuildActionsElement(x);
                case "":
                    throw new InvalidOperationException("No binding path or actions element was provided.");
            }

            var binding = new Binding {Path = bindingPath,};
            if (setupRowArguments.ShouldApplyFormatter(x))
            {
                binding.Converter = new UtcDateTimeToLocalStringConverter();
            }

            block.SetBinding(TextBlock.TextProperty, binding);
            return block;
        });
        stack.Children.AddRange(cells);
        setupRowArguments.DataGrid.RowDetailsTemplate = new DataTemplate(() => stack);
    }

    public static void SetupDataGridColumns(SetupColumnsArguments setupColumnsArguments)
    {
        IList<DataGridColumn> columns = new List<DataGridColumn>();
        foreach (int value in setupColumnsArguments.EnumNumbers)
        {
            string titleCaseEnum = setupColumnsArguments.GetEnumAsString(value).ScreamingSnakeCaseToTitleCase();
            bool isDateColumn = setupColumnsArguments.IsTemplateColumn(value);
            string bindingPath = setupColumnsArguments.GetBindingPath(value);
            DataGridColumn column;

            if (bindingPath == nameof(ISearchable.Id) && setupColumnsArguments.BuildActionsElement != null)
            {
                column = CreateDataGridActionsTemplateColumn(titleCaseEnum, value,
                    setupColumnsArguments.BuildActionsElement);
            }
            else if (bindingPath != string.Empty)
            {
                column = isDateColumn
                    ? CreateDataGridDateTemplateColumn(bindingPath, titleCaseEnum)
                    : CreateDataGridTextColumn(bindingPath, titleCaseEnum);
            }
            else
            {
                throw new InvalidOperationException("No binding path or actions element was provided.");
            }

            column.Width = new DataGridLength(setupColumnsArguments.GetColumnStarWidth(value),
                DataGridLengthUnitType.Star);

            columns.Add(column);
        }

        setupColumnsArguments.DataGrid.Columns.AddRange(columns);
    }

    private static DataGridColumn CreateDataGridActionsTemplateColumn(
        string header, int enumAsInt, Func<int, FrameworkElement> buildActionsElement)
    {
        return new DataGridTemplateColumn()
        {
            Header = header,
            CellTemplate = new DataTemplate(() => buildActionsElement(enumAsInt)),
        };
    }
}
