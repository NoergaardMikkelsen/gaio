namespace Statistics.Uno.Presentation.Factory;

public static class StackPanelFactory
{
    public static StackPanel CreateLabeledFieldPanel(string labelText, string placeholderText, string bindingPath)
    {
        var stackPanel = CreateDefaultPanel();

        var label = new TextBlock
        {
            Text = labelText,
            Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center,
        };
        var inputBox = new TextBox
        {
            Margin = new Thickness(5),
            VerticalAlignment = VerticalAlignment.Center,
            PlaceholderText = placeholderText,
        };
        inputBox.SetBinding(TextBox.TextProperty, new Binding
        {
            Path = new PropertyPath(bindingPath),
            Mode = BindingMode.TwoWay,
        });

        stackPanel.Children.Add(label);
        stackPanel.Children.Add(inputBox);

        return stackPanel;
    }

    public static StackPanel CreateDefaultPanel()
    {
        return new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(5),
        };
    }
}
