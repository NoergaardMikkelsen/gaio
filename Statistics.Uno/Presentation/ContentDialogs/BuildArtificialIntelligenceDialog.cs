using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs.ViewModel;
using Statistics.Uno.Presentation.Core;
using Statistics.Uno.Presentation.Core.Converters;

namespace Statistics.Uno.Presentation.ContentDialogs;

public class BuildArtificialIntelligenceDialog : ContentDialog
{
    public BuildArtificialIntelligenceDialog(IArtificialIntelligenceEndpoint aiEndpoint, IArtificialIntelligence? ai = null)
    {
        ai ??= new ArtificialIntelligence();

        DataContext = new BuildArtificialIntelligenceViewModel(ai);

        var logic = new BuildArtificialIntelligenceDialogLogic(ai, (BuildArtificialIntelligenceViewModel)DataContext, aiEndpoint);
        var ui = new BuildArtificialIntelligenceDialogUi(logic, (BuildArtificialIntelligenceViewModel)DataContext, this);

        this.Background(Theme.Brushes.Background.Default).Content(ui.CreateContentGrid());
    }

    private class BuildArtificialIntelligenceDialogUi : BaseUi<BuildArtificialIntelligenceDialogLogic, BuildArtificialIntelligenceViewModel>
    {
        private readonly ContentDialog dialog;

        public BuildArtificialIntelligenceDialogUi(
            BuildArtificialIntelligenceDialogLogic logic, BuildArtificialIntelligenceViewModel viewModel, ContentDialog dialog) : base(logic, viewModel)
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
            grid.RowDefinitions(Enumerable.Repeat(new GridLength(rowHeight, GridUnitType.Star), 7).ToArray());
            grid.ColumnDefinitions(Enumerable.Repeat(new GridLength(columnWidth, GridUnitType.Star), 2).ToArray());
        }

        /// <inheritdoc />
        protected override void AddControlsToGrid(Grid grid)
        {
            AddNameRow(grid);
            AddKeyRow(grid);
            AddAiTypeRow(grid);
            AddIdRow(grid);
            AddCreatedDateTimeRow(grid);
            AddUpdatedDateTimeRow(grid);
            AddVersionRow(grid);
        }

        private void AddNameRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Name:",
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
                PlaceholderText = "Enter AI name here...",
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildArtificialIntelligenceViewModel.Name)),
                Mode = BindingMode.TwoWay,
            });
            textBox.Grid(row: 0, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddKeyRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Key:",
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
                PlaceholderText = "Enter AI key here...",
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildArtificialIntelligenceViewModel.Key)),
                Mode = BindingMode.TwoWay,
            });
            textBox.Grid(row: 1, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddAiTypeRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "AI Type:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 2, column: 0);

            var comboBox = new ComboBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                ItemsSource = typeof(ArtificialIntelligenceType).EnumNamesToTitleCase().ToList(),
            };
            comboBox.SetBinding(Selector.SelectedIndexProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildArtificialIntelligenceViewModel.AiType)),
                Mode = BindingMode.TwoWay,
                Converter = new EnumToIntConverter(),
            });
            comboBox.Grid(row: 2, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(comboBox);
        }

        private void AddIdRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Id:",
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
            textBox.Text(() => ViewModel.Id);
            textBox.Grid(row: 3, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddCreatedDateTimeRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Created At:",
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
                IsEnabled = false
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildArtificialIntelligenceViewModel.CreatedDateTime)),
                Converter = new UtcDateTimeToLocalStringConverter()
            });
            textBox.Grid(row: 4, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddUpdatedDateTimeRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Last Updated At:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 5, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false
            };
            textBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(BuildArtificialIntelligenceViewModel.UpdatedDateTime)),
                Converter = new UtcDateTimeToLocalStringConverter()
            });
            textBox.Grid(row: 5, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }

        private void AddVersionRow(Grid grid)
        {
            var label = new TextBlock()
            {
                Text = "Version:",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 5),
            };
            label.Grid(row: 6, column: 0);

            var textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5),
                Width = 200,
                IsEnabled = false,
            };
            textBox.Text(() => ViewModel.Version);
            textBox.Grid(row: 6, column: 1);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
        }
    }

    private class BuildArtificialIntelligenceDialogLogic
    {
        private readonly BuildArtificialIntelligenceViewModel viewModel;
        private readonly IArtificialIntelligenceEndpoint aiEndpoint;
        private readonly IArtificialIntelligence ai;

        public BuildArtificialIntelligenceDialogLogic(IArtificialIntelligence ai, BuildArtificialIntelligenceViewModel viewModel, IArtificialIntelligenceEndpoint aiEndpoint)
        {
            this.viewModel = viewModel;
            this.aiEndpoint = aiEndpoint;
            this.ai = ai;
        }

        public async void PrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!HasChanges())
            {
                return;
            }

            CollectUpdatedFieldsFromViewModel();

            if (ai.Id == 0)
            {
                await aiEndpoint.AddSingle(CancellationToken.None, (ArtificialIntelligence)ai);
            }
            else
            {
                await aiEndpoint.UpdateSingle(CancellationToken.None, (ArtificialIntelligence)ai);
            }
        }

        private void CollectUpdatedFieldsFromViewModel()
        {
            ai.Name = viewModel.Name;
            ai.Key = viewModel.Key;
            ai.AiType = viewModel.AiType;
        }

        private bool HasChanges()
        {
            return ai.Name != viewModel.Name || ai.Key != viewModel.Key || ai.AiType != viewModel.AiType;
        }
    }
}
