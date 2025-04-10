using Moq;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Services.ArtificialIntelligence;

namespace Statistics.Tests.Services;

[TestFixture]
public class MasterArtificialIntelligencePromptServiceTests
{
    private Mock<IArtificialIntelligencePromptService> mockOpenAiPromptService;
    private MasterArtificialIntelligencePromptService service;

    [SetUp]
    public void SetUp()
    {
        mockOpenAiPromptService = new Mock<IArtificialIntelligencePromptService>();
        var promptServices = new Dictionary<ArtificialIntelligenceType, IArtificialIntelligencePromptService>
        {
            {ArtificialIntelligenceType.OPEN_AI_NO_WEB, mockOpenAiPromptService.Object},
        };

        service = new MasterArtificialIntelligencePromptService(promptServices);
    }

    [Test]
    public async Task PromptSuppliedAis_WithMultiplePrompts_ShouldReturnFlattenedResponses()
    {
        // Arrange
        var ai = new Mock<IArtificialIntelligence>();
        ai.SetupGet(a => a.AiType).Returns(ArtificialIntelligenceType.OPEN_AI_NO_WEB);

        var prompt1 = new Mock<IPrompt>();
        var prompt2 = new Mock<IPrompt>();

        var response1 = new Mock<IResponse>();
        var response2 = new Mock<IResponse>();

        mockOpenAiPromptService
            .Setup(s => s.ExecutePrompts(It.IsAny<IArtificialIntelligence>(), It.IsAny<IEnumerable<IPrompt>>()))
            .ReturnsAsync(new List<IResponse> {response1.Object, response2.Object,});

        // Act
        var result = await service.PromptSuppliedAis(new List<IArtificialIntelligence> {ai.Object,},
            new List<IPrompt> {prompt1.Object, prompt2.Object,});

        // Assert
        result.Should().Contain([response1.Object, response2.Object,]);
    }

    [Test]
    public async Task PromptSuppliedAis_WithSinglePrompt_ShouldReturnResponses()
    {
        // Arrange
        var ai = new Mock<IArtificialIntelligence>();
        ai.SetupGet(a => a.AiType).Returns(ArtificialIntelligenceType.OPEN_AI_NO_WEB);

        var prompt = new Mock<IPrompt>();

        var response = new Mock<IResponse>();

        mockOpenAiPromptService.Setup(s => s.ExecutePrompt(It.IsAny<IArtificialIntelligence>(), It.IsAny<IPrompt>()))
            .ReturnsAsync(response.Object);

        // Act
        var result = await service.PromptSuppliedAis(new List<IArtificialIntelligence> {ai.Object,}, prompt.Object);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(response.Object);
    }
}
