namespace Statistics.Uno;

public sealed partial class DefaultPage : Page
{
    public DefaultPage()
    {
        this.Background(Theme.Brushes.Background.Default).Content(new StackPanel()
            .VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Children(
                new TextBlock().Text(
                    $"If you are seeing this, then some navigation failed, or was not fully implemented." +
                    $"{Environment.NewLine}Also hello from Uno Platform!")));
    }
}
