using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
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

        var logic = new BuildPromptDialogLogic(prompt, (BuildPromptViewModel) DataContext, promptEndpoint);
        var ui = new BuildPromptDialogUi(logic, (BuildPromptViewModel) DataContext, this);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildPromptDialogUi : BaseUi<BuildPromptDialogLogic, BuildPromptViewModel>
    {
        private readonly ContentDialog dialog;

        public BuildPromptDialogUi(
            BuildPromptDialogLogic logic, BuildPromptViewModel viewModel, ContentDialog dialog) : base(logic, viewModel)
        {
            this.dialog = dialog;

            this.dialog.PrimaryButtonClick += Logic.PrimaryButtonClicked;
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
            AddTextRow(grid);
            AddIdRow(grid);
            AddCreatedDateTimeRow(grid);
            AddUpdatedDateTimeRow(grid);
            AddVersionRow(grid);
        }

        private void AddVersionRow(Grid grid)
        {
            // TODO: Add Tooltips to the label
            var label = new TextBlock()
            {
                Text = "Version:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 4, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false,
            };
            textBox.Text(() => ViewModel.Version);
            textBox.Grid(row: 4, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddUpdatedDateTimeRow(Grid grid)
        {
            // TODO: Add Tooltips to the label
            var label = new TextBlock()
            {
                Text = "Last Updated At:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 3, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false,
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildPromptViewModel.UpdatedDateTime)),
                Converter = new UtcDateTimeToLocalStringConverter(),
            });
            textBox.Grid(row: 3, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddCreatedDateTimeRow(Grid grid)
        {
            // TODO: Add Tooltips to the label
            var label = new TextBlock()
            {
                Text = "Created At:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 2, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false,
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildPromptViewModel.CreatedDateTime)),
                Converter = new UtcDateTimeToLocalStringConverter(),
            });
            textBox.Grid(row: 2, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddIdRow(Grid grid)
        {
            // TODO: Add Tooltips to the label
            var label = new TextBlock()
            {
                Text = "Id:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 1, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false,
            };
            textBox.Text(() => ViewModel.Id);
            textBox.Grid(row: 1, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddTextRow(Grid grid)
        {
            // TODO: Add Tooltips to the label
            var label = new TextBlock()
            {
                Text = "Text:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 0, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                PlaceholderText = "Enter prompt text here...",
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildPromptViewModel.Text)),
                Mode = BindingMode.TwoWay,
            });
            textBox.Grid(row: 0, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
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

        public async void PrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!HasChanges())
            {
                return;
            }

            CollectUpdatedFieldsFromViewModel();

            if (prompt.Id == 0)
            {
                await promptEndpoint.AddSingle(CancellationToken.None, (Prompt) prompt);
            }
            else
            {
                await promptEndpoint.UpdateSingle(CancellationToken.None, (Prompt) prompt);
            }
        }

        private void CollectUpdatedFieldsFromViewModel()
        {
            prompt.Text = viewModel.Text;
        }

        private bool HasChanges()
        {
            return prompt.Text != viewModel.Text;
        }
    }
}
