using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.WinUI.UI.Controls;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Extensions;

namespace Statistics.Uno.Presentation.Core;

public abstract class BaseLogic<TItem>
{
    protected abstract ObservableCollection<TItem> GetCollection();
    protected abstract string GetPropertyNameFromColumnHeader(string header);
    internal abstract Task UpdateDisplayedItems(bool forceUpdate = false);

    public void SortItems(object? sender, DataGridColumnEventArgs e)
    {
        if (sender is not DataGrid dataGrid || e.Column == null)
            return;

        string? propertyName =
            GetPropertyNameFromColumnHeader(e.Column.Header.ToString() ?? throw new InvalidOperationException());

        // Determine the sort direction
        DataGridSortDirection sortDirection = e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

        dataGrid.Columns.ForEach(column => column.SortDirection = null);

        // Update the column's sort direction
        e.Column.SortDirection = sortDirection;

        // Perform the sorting
        var collection = GetCollection();
        var sortedItems = sortDirection == DataGridSortDirection.Ascending
            ? collection.OrderBy(item => GetPropertyValue(item, propertyName)).ToList()
            : collection.OrderByDescending(item => GetPropertyValue(item, propertyName)).ToList();

        // Update the ObservableCollection
        collection.Clear();
        foreach (TItem? item in sortedItems)
        {
            collection.Add(item);
        }
    }

    private object? GetPropertyValue(object obj, string propertyName)
    {
        if (!propertyName.Contains('.'))
        {
            // Handle direct property
            return obj.GetSortableValue(propertyName);
        }

        // Handle child object property
        string[]? parts = propertyName.Split('.');
        PropertyInfo? parentProperty = obj.GetType().GetProperty(parts[0]);
        object? childObject = parentProperty?.GetValue(obj);
        return childObject?.GetSortableValue(parts[1]);

    }

    public async void SearchFieldChanged(object? sender, string e)
    {
        if (e.Length < 5)
        {
            return;
        }

        await UpdateDisplayedItems();
    }
}
