using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs.ViewModel;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.ContentDialogs;

public class BuildPromptDialog : ContentDialog
{
    public BuildPromptDialog(IPromptEndpoint promptEndpoint, IPrompt? prompt = null)
    {
        prompt ??= new Prompt();

        DataContext = new BuildPromptViewModel(prompt);

        var logic = new BuildPromptDialogLogic(prompt, (BuildPromptViewModel)DataContext, promptEndpoint);
        var ui = new BuildPromptDialogUi(logic, (BuildPromptViewModel)DataContext, this);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildPromptDialogUi : BaseDialogUi<BuildPromptDialogLogic, BuildPromptViewModel, Prompt, SearchablePrompt>
    {
        public BuildPromptDialogUi(
            BuildPromptDialogLogic logic, BuildPromptViewModel viewModel, ContentDialog dialog) : base(logic, viewModel, dialog)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
        {
            const int rowHeight = 20;
            const int columnWidth = 50;

            grid.SafeArea(SafeArea.InsetMask.VisibleBounds);
            grid.RowDefinitions(Enumerable.Repeat(new GridLength(rowHeight, GridUnitType.Star), 5).ToArray());
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 2).ToArray());
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            var dateTimeConverter = new UtcDateTimeToLocalStringConverter();

            AddLabelAndTextBox(grid, "Text:", nameof(BuildPromptViewModel.Text), 0,
                placeholderText: "Enter prompt text here...");
            AddLabelAndTextBox(grid, "Id:", nameof(BuildPromptViewModel.Id), 1, isEnabled: false);
            AddLabelAndTextBox(grid, "Created At:", nameof(BuildPromptViewModel.CreatedDateTime), 2, isEnabled: false,
                converter: dateTimeConverter);
            AddLabelAndTextBox(grid, "Last Updated At:", nameof(BuildPromptViewModel.UpdatedDateTime), 3,
                isEnabled: false, converter: dateTimeConverter);
            AddLabelAndTextBox(grid, "Version:", nameof(BuildPromptViewModel.Version), 4, isEnabled: false);
        }


    }

    private class BuildPromptDialogLogic : BaseDialogLogic<BuildPromptViewModel, Prompt, SearchablePrompt>
    {
        public BuildPromptDialogLogic(IPrompt prompt, BuildPromptViewModel viewModel, IPromptEndpoint promptEndpoint)
            : base((Prompt) prompt, viewModel, promptEndpoint)
        {
        }

        protected override void CollectUpdatedFieldsFromViewModel()
        {
            entity.Text = viewModel.Text;
        }

        protected override bool HasChanges()
        {
            return entity.Text != viewModel.Text;
        }
    }
}
