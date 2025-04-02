using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;
using Statistics.Uno.Endpoints;
using Statistics.Uno.Presentation.ContentDialogs;

namespace Statistics.Uno.Presentation.Factory;

public static class ContentDialogFactory
{
    public static async Task<ContentDialogResult> ShowConfirmationDialog(string title, string content, XamlRoot? root)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            XamlRoot = root,
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildPromptDialogFromNew(
        IPromptEndpoint promptEndpoint, XamlRoot? root)
    {
        var dialog = new BuildPromptDialog(promptEndpoint)
        {
            Title = "New Prompt",
            XamlRoot = root,
            PrimaryButtonText = "Create New",
            CloseButtonText = "Cancel Creation",
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildPromptDialogFromExisting(
        IPromptEndpoint promptEndpoint, IPrompt prompt, XamlRoot? root)
    {
        var dialog = new BuildPromptDialog(promptEndpoint, prompt)
        {
            Title = "Edit Prompt",
            XamlRoot = root,
            PrimaryButtonText = "Save Changes",
            CloseButtonText = "Discard Changes",
        };
        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildArtificialIntelligenceDialogFromNew(
        IArtificialIntelligenceEndpoint aiEndpoint, XamlRoot? root)
    {
        var dialog = new BuildArtificialIntelligenceDialog(aiEndpoint)
        {
            Title = "New Artificial Intelligence",
            XamlRoot = root,
            PrimaryButtonText = "Create New",
            CloseButtonText = "Cancel Creation",
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildArtificialIntelligenceDialogFromExisting(
        IArtificialIntelligenceEndpoint aiEndpoint, IArtificialIntelligence ai, XamlRoot? root)
    {
        var dialog = new BuildArtificialIntelligenceDialog(aiEndpoint, ai)
        {
            Title = "Edit Artificial Intelligence",
            XamlRoot = root,
            PrimaryButtonText = "Save Changes",
            CloseButtonText = "Discard Changes",
        };
        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildKeywordDialogFromExisting(IKeywordEndpoint keywordApi, IKeyword keyword, XamlRoot? root)
    {
        var dialog = new BuildKeywordDialog(keywordApi, keyword)
        {
            Title = "Edit Keyword",
            XamlRoot = root,
            PrimaryButtonText = "Save Changes",
            CloseButtonText = "Discard Changes",
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowBuildKeywordDialogFromNew(IKeywordEndpoint keywordApi, XamlRoot? root)
    {
        var dialog = new BuildKeywordDialog(keywordApi)
        {
            Title = "New Keyword",
            XamlRoot = root,
            PrimaryButtonText = "Create New",
            CloseButtonText = "Cancel Creation",
        };

        return await dialog.ShowAsync();
    }
}
