using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Extensions;

public static class DataGridFactory
{
    private const string DATE_FORMATTER = "{0:dd/MM/yyyy HH:mm:ss}";

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
        };

        setupColumns(dataGrid);
        setupRowTemplate(dataGrid);

        dataGrid.SetBinding(DataGrid.ItemsSourceProperty,
            new Binding() {Path = itemsSourcePath, Source = dataContext,});

        return dataGrid;
    }

    private static DataGridTemplateColumn CreateDataGridTemplateColumn(
        string bindingPath, string header, bool shouldApplyFormatter)
    {
        var textBlock = new TextBlock()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0),
        };

        var binding = new Binding {Path = bindingPath,};
        if (shouldApplyFormatter)
        {
            binding.Converter = new StringFormatConverter();
            binding.ConverterParameter = DATE_FORMATTER;
        }

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

    public static void SetupDataGridRowTemplate(
        DataGrid dataGrid, IEnumerable<int> enumNumbers, Func<int, bool> shouldApplyFormatter,
        Func<int, string> getBindingPath)
    {
        var stack = new StackPanel();
        var cells = enumNumbers.Select(x =>
        {
            var block = new TextBlock() {Margin = new Thickness(10),};
            var binding = new Binding {Path = getBindingPath(x),};
            if (shouldApplyFormatter(x))
            {
                binding.Converter = new StringFormatConverter();
                binding.ConverterParameter = DATE_FORMATTER;
            }

            block.SetBinding(TextBlock.TextProperty, binding);
            return block;
        });
        stack.Children.AddRange(cells);
        dataGrid.RowDetailsTemplate = new DataTemplate(() => stack);
    }

    public static void SetupDataGridColumns(
        DataGrid dataGrid, IEnumerable<int> enumNumbers, Func<int, string> getBindingPath,
        Func<int, bool> isTemplateColumn, Func<int, string> getEnumAsString)
    {
        IList<DataGridColumn> columns = new List<DataGridColumn>();
        foreach (int value in enumNumbers)
        {
            string titleCaseEnum = getEnumAsString(value).ScreamingSnakeCaseToTitleCase();
            bool isTemplateColumnManifested = isTemplateColumn(value);
            string bindingPath = getBindingPath(value);

            DataGridColumn column = isTemplateColumnManifested
                ? CreateDataGridTemplateColumn(bindingPath, titleCaseEnum, true)
                : CreateDataGridTextColumn(bindingPath, titleCaseEnum);

            column.Width = new DataGridLength(isTemplateColumnManifested ? 1 : 2, DataGridLengthUnitType.Star);
            columns.Add(column);
        }

        dataGrid.Columns.AddRange(columns);
    }
}
