using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs.ViewModel;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.ContentDialogs;

public class BuildArtificialIntelligenceDialog : ContentDialog
{
    public BuildArtificialIntelligenceDialog(
        IArtificialIntelligenceEndpoint aiEndpoint, IArtificialIntelligence? ai = null)
    {
        ai ??= new ArtificialIntelligence();

        DataContext = new BuildArtificialIntelligenceViewModel(ai);

        var logic = new BuildArtificialIntelligenceDialogLogic(ai, (BuildArtificialIntelligenceViewModel) DataContext,
            aiEndpoint);
        var ui = new BuildArtificialIntelligenceDialogUi(logic, (BuildArtificialIntelligenceViewModel) DataContext,
            this);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildArtificialIntelligenceDialogUi : BaseDialogUi<BuildArtificialIntelligenceDialogLogic,
        BuildArtificialIntelligenceViewModel, ArtificialIntelligence, SearchableArtificialIntelligence>
    {
        public BuildArtificialIntelligenceDialogUi(
            BuildArtificialIntelligenceDialogLogic logic, BuildArtificialIntelligenceViewModel viewModel,
            ContentDialog dialog) : base(logic, viewModel, dialog)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
        {
            const int rowHeight = 20;
            const int columnWidth = 50;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(Enumerable.Repeat(new GridLength(rowHeight, GridUnitType.Star), 7).ToArray());
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 2).ToArray());
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            var dateTimeConverter = new UtcDateTimeToLocalStringConverter();

            AddLabelAndTextBox(grid, "Name:", nameof(BuildArtificialIntelligenceViewModel.Name), 0,
                placeholderText: "Enter AI name here...");
            AddLabelAndTextBox(grid, "Key:", nameof(BuildArtificialIntelligenceViewModel.Key), 1,
                placeholderText: "Enter AI key here...");
            AddLabelAndComboBox(grid, "AI Type:", nameof(BuildArtificialIntelligenceViewModel.AiType), 2,
                typeof(ArtificialIntelligenceType).EnumNamesToTitleCase().ToList(), new EnumToIntConverter());
            AddLabelAndTextBox(grid, "Id:", nameof(BuildArtificialIntelligenceViewModel.Id), 3, false);
            AddLabelAndTextBox(grid, "Created At:", nameof(BuildArtificialIntelligenceViewModel.CreatedDateTime), 4,
                false, dateTimeConverter);
            AddLabelAndTextBox(grid, "Last Updated At:", nameof(BuildArtificialIntelligenceViewModel.UpdatedDateTime),
                5, false, dateTimeConverter);
            AddLabelAndTextBox(grid, "Version:", nameof(BuildArtificialIntelligenceViewModel.Version), 6, false);
        }
    }

    private class BuildArtificialIntelligenceDialogLogic : BaseDialogLogic<BuildArtificialIntelligenceViewModel,
        ArtificialIntelligence, SearchableArtificialIntelligence>
    {
        public BuildArtificialIntelligenceDialogLogic(
            IArtificialIntelligence ai, BuildArtificialIntelligenceViewModel viewModel,
            IArtificialIntelligenceEndpoint aiEndpoint) : base((ArtificialIntelligence) ai, viewModel, aiEndpoint)
        {
        }

        protected override void CollectUpdatedFieldsFromViewModel()
        {
            entity.Name = viewModel.Name;
            entity.Key = viewModel.Key;
            entity.AiType = viewModel.AiType;
        }

        protected override bool HasChanges()
        {
            return entity.Name != viewModel.Name || entity.Key != viewModel.Key || entity.AiType != viewModel.AiType;
        }
    }
}
