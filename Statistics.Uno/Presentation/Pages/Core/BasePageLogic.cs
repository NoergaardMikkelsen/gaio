namespace Statistics.Uno.Presentation.Pages.Core;

public abstract class BasePageLogic
{
    protected async Task<ContentDialogResult> ShowConfirmationDialog(string title, string content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
        };

        Console.WriteLine("ShowConfirmationDialog");
        return await dialog.ShowAsync();
    }
}
