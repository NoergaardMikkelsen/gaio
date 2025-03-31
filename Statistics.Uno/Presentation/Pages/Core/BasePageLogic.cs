namespace Statistics.Uno.Presentation.Pages.Core;

public abstract class BasePageLogic
{
    private readonly Page parentPage;

    protected BasePageLogic(Page parentPage)
    {
        this.parentPage = parentPage;
    }

    protected async Task<ContentDialogResult> ShowConfirmationDialog(string title, string content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            XamlRoot = parentPage.XamlRoot,
        };

        Console.WriteLine("ShowConfirmationDialog");
        return await dialog.ShowAsync();
    }
}
