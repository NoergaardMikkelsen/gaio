using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Services.Keywords;

namespace Statistics.Tests.Services;

[TestFixture]
public class AppliedKeywordServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _service = new AppliedKeywordService();
    }

    private IAppliedKeywordService _service;

    private static IEnumerable<IResponse> CreateResponses(
        string matchingText, DateTime? matchingCreatedDateTime = null, DateTime? nonMatchingCreatedDateTime = null)
    {
        var responses = Enum.GetValues<ArtificialIntelligenceType>().Select(aiType => new Response
        {
            Text = matchingText,
            Ai = new ArtificialIntelligence {AiType = aiType,},
            CreatedDateTime = matchingCreatedDateTime ?? DateTime.UtcNow,
        }).ToList();

        responses.Add(new Response
        {
            Text = "non-matching response",
            Ai = new ArtificialIntelligence {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,},
            CreatedDateTime = nonMatchingCreatedDateTime ?? DateTime.UtcNow.AddDays(-2),
        });

        return responses;
    }

    [Test]
    public async Task GenerateAppliedKeywordUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true,};
        var responses = CreateResponses("test response").ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false,};
        var responses = CreateResponses("test response").ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true, StartSearch = DateTime.UtcNow.AddDays(-1),};
        var responses = CreateResponses("test response", DateTime.UtcNow).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false, StartSearch = DateTime.UtcNow.AddDays(-1),};
        var responses = CreateResponses("test response", DateTime.UtcNow).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true, EndSearch = DateTime.UtcNow.AddDays(-1),};
        var responses = CreateResponses("test response", DateTime.UtcNow.AddDays(-2)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false, EndSearch = DateTime.UtcNow.AddDays(-1),};
        var responses = CreateResponses("test response", DateTime.UtcNow.AddDays(-2)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordForTimeRangeUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword
        {
            Text = "test",
            UseRegex = true,
            StartSearch = DateTime.UtcNow.AddDays(-2),
            EndSearch = DateTime.UtcNow.AddDays(-1),
        };
        var responses = CreateResponses("test response", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))
            .ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordForTimeRangeUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword
        {
            Text = "test",
            UseRegex = false,
            StartSearch = DateTime.UtcNow.AddDays(-2),
            EndSearch = DateTime.UtcNow.AddDays(-1),
        };
        var responses = CreateResponses("test response", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))
            .ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword,}, responses);

        // Assert
        foreach (ArtificialIntelligenceType aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType),
            });
        }
    }
}
