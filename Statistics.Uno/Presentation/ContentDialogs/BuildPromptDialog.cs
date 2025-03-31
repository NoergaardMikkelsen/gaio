using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs.ViewModel;
using Statistics.Uno.Presentation.Core;

namespace Statistics.Uno.Presentation.ContentDialogs;

public class BuildPromptDialog : ContentDialog
{
    public BuildPromptDialog(IPromptEndpoint promptEndpoint, IPrompt? prompt = null)
    {
        prompt ??= new Prompt();

        DataContext = new BuildPromptViewModel(prompt);

        var logic = new BuildPromptDialogLogic(prompt, (BuildPromptViewModel) DataContext, promptEndpoint);
        var ui = new BuildPromptDialogUi(logic, (BuildPromptViewModel) DataContext);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildPromptDialogUi : BaseUi<BuildPromptDialogLogic, BuildPromptViewModel>
    {
        public BuildPromptDialogUi(BuildPromptDialogLogic logic, BuildPromptViewModel viewModel) : base(logic,
            viewModel)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureGrid(Grid grid)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            throw new NotImplementedException();
        }
    }

    private class BuildPromptDialogLogic
    {
        private readonly BuildPromptViewModel viewModel;
        private readonly IPromptEndpoint promptEndpoint;
        private readonly IPrompt prompt;

        public BuildPromptDialogLogic(IPrompt prompt, BuildPromptViewModel viewModel, IPromptEndpoint promptEndpoint)
        {
            this.viewModel = viewModel;
            this.promptEndpoint = promptEndpoint;
            this.prompt = prompt;
        }
    }
}
