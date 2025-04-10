using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Statistics.Api.Controllers;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Tests.Controllers;

[TestFixture]
public class ArtificialIntelligenceControllerTests
{
    private Mock<IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence>> _mockEntityService;
    private Mock<ILogger<ArtificialIntelligenceController>> _mockLogger;
    private TestableArtificialIntelligenceController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockEntityService = new Mock<IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence>>();
        _mockLogger = new Mock<ILogger<ArtificialIntelligenceController>>();
        _controller = new TestableArtificialIntelligenceController(_mockEntityService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResultWithEntities()
    {
        // Arrange
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new ArtificialIntelligence(1) { Name = "AI1" },
            new ArtificialIntelligence(2) { Name = "AI2" }
        };
        _mockEntityService
            .Setup(service => service.GetEntities(It.IsAny<SearchableArtificialIntelligence>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAll() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task GetEntitiesByComplexQuery_WithValidSearchableArtificialIntelligence_ReturnsEntities()
    {
        // Arrange
        var searchableAI = new SearchableArtificialIntelligence { Name = "TestAI" };
        var complexSearchable = new ComplexSearchable { SearchableArtificialIntelligence = searchableAI };
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new ArtificialIntelligence(1) { Name = "AI1" },
            new ArtificialIntelligence(2) { Name = "AI2" }
        };

        _mockEntityService
            .Setup(service => service.GetEntities(searchableAI))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public void GetEntitiesByComplexQuery_WithNullSearchableArtificialIntelligence_ThrowsArgumentNullException()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable { SearchableArtificialIntelligence = null };

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchableArtificialIntelligence')");
    }

    // Private wrapper class to expose the protected method
    private class TestableArtificialIntelligenceController : ArtificialIntelligenceController
    {
        public TestableArtificialIntelligenceController(
            IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> entityService,
            ILogger<ArtificialIntelligenceController> logger)
            : base(entityService, logger)
        {
        }

        public Task<IEnumerable<ArtificialIntelligence>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
