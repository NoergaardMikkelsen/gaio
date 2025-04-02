using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs.ViewModel;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.ContentDialogs;

public class BuildKeywordDialog : ContentDialog
{
    public BuildKeywordDialog(IKeywordEndpoint keywordEndpoint, IKeyword? keyword = null)
    {
        keyword ??= new Keyword();

        DataContext = new BuildKeywordViewModel(keyword);

        var logic = new BuildKeywordDialogLogic(keyword, (BuildKeywordViewModel)DataContext, keywordEndpoint);
        var ui = new BuildKeywordDialogUi(logic, (BuildKeywordViewModel)DataContext, this);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildKeywordDialogUi : BaseDialogUi<BuildKeywordDialogLogic, BuildKeywordViewModel, Keyword, SearchableKeyword>
    {
        public BuildKeywordDialogUi(
            BuildKeywordDialogLogic logic, BuildKeywordViewModel viewModel, ContentDialog dialog) : base(logic, viewModel, dialog)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
        {
            const int rowHeight = 20;
            const int columnWidth = 100;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(Enumerable.Repeat(new GridLength(rowHeight, GridUnitType.Star), 6).ToArray());
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 2).ToArray());
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            var dateTimeConverter = new UtcDateTimeToLocalStringConverter();

            AddLabelAndTextBox(grid, "Text:", nameof(BuildKeywordViewModel.Text), 0,
                placeholderText: "Enter keyword text here...");
            AddLabelAndCheckBox(grid, "Use Regex:", nameof(BuildKeywordViewModel.UseRegex), 1);
            AddLabelAndTextBox(grid, "Id:", nameof(BuildKeywordViewModel.Id), 2, isEnabled: false);
            AddLabelAndTextBox(grid, "Created At:", nameof(BuildKeywordViewModel.CreatedDateTime), 3, isEnabled: false,
                converter: dateTimeConverter);
            AddLabelAndTextBox(grid, "Last Updated At:", nameof(BuildKeywordViewModel.UpdatedDateTime), 4,
                isEnabled: false, converter: dateTimeConverter);
            AddLabelAndTextBox(grid, "Version:", nameof(BuildKeywordViewModel.Version), 5, isEnabled: false);
        }

    }

    private class BuildKeywordDialogLogic : BaseDialogLogic<BuildKeywordViewModel, Keyword, SearchableKeyword>
    {
        public BuildKeywordDialogLogic(IKeyword keyword, BuildKeywordViewModel viewModel, IKeywordEndpoint keywordEndpoint)
            : base((Keyword) keyword, viewModel, keywordEndpoint)
        {
        }

        protected override void CollectUpdatedFieldsFromViewModel()
        {
            entity.Text = viewModel.Text;
            entity.UseRegex = viewModel.UseRegex;
        }

        protected override bool HasChanges()
        {
            return entity.Text != viewModel.Text || entity.UseRegex != viewModel.UseRegex;
        }
    }
}
