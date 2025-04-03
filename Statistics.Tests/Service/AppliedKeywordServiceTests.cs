using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Services.Keywords;

namespace Statistics.Tests.Service
{
    [TestFixture]
    public class AppliedKeywordServiceTests
    {
        private IAppliedKeywordService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new AppliedKeywordService();
        }

        [Test]
        public async Task GenerateAppliedKeywordUsingRegex_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = true,};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,},},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,},},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordUsingContains_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = false,};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,},},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,},},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordWithStartOnlyUsingRegex_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = true, StartSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordWithStartOnlyUsingContains_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = false, StartSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordWithEndOnlyUsingRegex_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = true, EndSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordWithEndOnlyUsingContains_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = false, EndSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordForTimeRangeUsingRegex_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = true, StartSearch = DateTime.UtcNow.AddDays(-2), EndSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }

        [Test]
        public async Task GenerateAppliedKeywordForTimeRangeUsingContains_ShouldReturnCorrectResult()
        {
            // Arrange
            var keyword = new Keyword { Text = "test", UseRegex = false, StartSearch = DateTime.UtcNow.AddDays(-2), EndSearch = DateTime.UtcNow.AddDays(-1),};
            var responses = new List<IResponse>
            {
                new Response { Text = "test response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow.AddDays(-2),},
                new Response { Text = "another response", Ai = new ArtificialIntelligence() { AiType = ArtificialIntelligenceType.OPEN_AI,}, CreatedDateTime = DateTime.UtcNow,},
            };

            // Act
            var result = await _service.CalculateAppliedKeywords([keyword,], responses);

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    Text = "test",
                    AiType = ArtificialIntelligenceType.OPEN_AI,
                    MatchingResponsesCount = 1,
                    TotalResponsesCount = 2,
                });
        }
    }
}
