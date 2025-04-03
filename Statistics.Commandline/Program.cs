using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Commandline;

internal class Program
{
    private CommandlineStartup Startup { get; set; }

    private static void Main(string[] args)
    {
        var program = new Program();

        program.Run(args).GetAwaiter().GetResult();
        Console.ReadKey();
    }

    private async Task Run(string[] args)
    {
        Startup = new CommandlineStartup();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        Startup.SetupApplication(builder);
        Startup.SetupServices(builder.Services);

        using IHost host = builder.Build();
        Console.WriteLine("Host has been build...");

        await ExecuteAllPrompts();

        await host.RunAsync();
    }

    private async Task ExecuteAllPrompts()
    {
        GetServicesOrThrow(out var promptEntityService, out var aiEntityService, out var responseEntityService,
            out IMasterArtificialIntelligencePromptService masterAiService);

        var allAis = await aiEntityService.GetEntities(new SearchableArtificialIntelligence());
        var artificialIntelligences = allAis.ToList();
        if (!artificialIntelligences.Any())
        {
            return;
        }

        var allPrompts = await promptEntityService.GetEntities(new SearchablePrompt());
        var prompts = allPrompts.ToList();
        if (!prompts.Any())
        {
            return;
        }

        var aiResponses = await masterAiService.PromptSuppliedAis(artificialIntelligences, prompts);
        var responses = aiResponses.ToList();
        if (!responses.Any())
        {
            return;
        }

        await responseEntityService.AddEntities(responses.Cast<Response>().ToList());
    }

    private void GetServicesOrThrow(
        out IEntityQueryService<Prompt, SearchablePrompt> promptEntityService,
        out IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> aiEntityService,
        out IEntityQueryService<Response, SearchableResponse> responseEntityService,
        out IMasterArtificialIntelligencePromptService masterAiService)
    {
        promptEntityService = Startup.ServiceProvider.GetService<IEntityQueryService<Prompt, SearchablePrompt>>() ??
                              throw new InvalidOperationException();
        aiEntityService = Startup.ServiceProvider
                              .GetService<
                                  IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence>>() ??
                          throw new InvalidOperationException();
        responseEntityService =
            Startup.ServiceProvider.GetService<IEntityQueryService<Response, SearchableResponse>>() ??
            throw new InvalidOperationException();
        masterAiService = Startup.ServiceProvider.GetService<IMasterArtificialIntelligencePromptService>() ??
                          throw new InvalidOperationException();
    }
}
