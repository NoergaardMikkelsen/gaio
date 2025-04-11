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
    [SetUp]
    public void SetUp()
    {
        _mockEntityService = new Mock<IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence>>();
        _mockLogger = new Mock<ILogger<ArtificialIntelligenceController>>();
        _controller = new TestableArtificialIntelligenceController(_mockEntityService.Object, _mockLogger.Object);
    }

    private Mock<IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence>> _mockEntityService;
    private Mock<ILogger<ArtificialIntelligenceController>> _mockLogger;
    private TestableArtificialIntelligenceController _controller;

    [Test]
    public async Task GetAll_ReturnsOkResultWithEntities()
    {
        // Arrange
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "AI1",},
            new(2) {Name = "AI2",},
        };
        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableArtificialIntelligence>()))
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
        var searchableAI = new SearchableArtificialIntelligence {Name = "TestAI",};
        var complexSearchable = new ComplexSearchable {SearchableArtificialIntelligence = searchableAI,};
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "AI1",},
            new(2) {Name = "AI2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(searchableAI)).ReturnsAsync(expectedEntities);

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
        var complexSearchable = new ComplexSearchable {SearchableArtificialIntelligence = null,};

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchableArtificialIntelligence')");
    }

    [Test]
    public async Task GetAllByComplexQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable
        {
            SearchableArtificialIntelligence = new SearchableArtificialIntelligence {Name = "TestAI",},
        };
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "AI1",},
            new(2) {Name = "AI2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableArtificialIntelligence>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAllByComplexQuery(complexSearchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }


    [Test]
    public async Task GetById_ReturnsOkResultWithEntity()
    {
        // Arrange
        var id = 1;
        var expectedEntity = new ArtificialIntelligence(id) {Name = "AI1",};
        _mockEntityService.Setup(service => service.GetEntity(It.Is<SearchableArtificialIntelligence>(s => s.Id == id)))
            .ReturnsAsync(expectedEntity);

        // Act
        var result = await _controller.GetById(id) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntity);
    }

    [Test]
    public async Task AddSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new ArtificialIntelligence(1) {Name = "AI1",};

        // Act
        var result = await _controller.AddSingle(entity) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntity(entity, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task DeleteById_ReturnsOkResult()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _controller.DeleteById(id) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.DeleteEntityById(id, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetByQuery_ReturnsOkResultWithEntity()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {Name = "TestAI",};
        var expectedEntity = new ArtificialIntelligence(1) {Name = "AI1",};
        _mockEntityService.Setup(service => service.GetEntity(searchable)).ReturnsAsync(expectedEntity);

        // Act
        var result = await _controller.GetByQuery(searchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntity);
    }

    [Test]
    public async Task GetAllByQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {Name = "TestAI",};
        var expectedEntities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "AI1",},
            new(2) {Name = "AI2",},
        };
        _mockEntityService.Setup(service => service.GetEntities(searchable)).ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAllByQuery(searchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task UpdateMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "UpdatedAI1",},
            new(2) {Name = "UpdatedAI2",},
        };

        // Act
        var result = await _controller.UpdateMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task AddMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<ArtificialIntelligence>
        {
            new(1) {Name = "AI1",},
            new(2) {Name = "AI2",},
        };

        // Act
        var result = await _controller.AddMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task UpdateSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new ArtificialIntelligence(1) {Name = "UpdatedAI",};

        // Act
        var result = await _controller.UpdateSingle(entity) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntity(entity, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task DeleteByQuery_ReturnsOkResult()
    {
        // Arrange
        var searchable = new SearchableArtificialIntelligence {Name = "TestAI",};

        // Act
        var result = await _controller.DeleteByQuery(searchable) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.DeleteEntity(searchable, It.IsAny<bool>()), Times.Once);
    }


    private class TestableArtificialIntelligenceController : ArtificialIntelligenceController
    {
        public TestableArtificialIntelligenceController(
            IEntityQueryService<ArtificialIntelligence, SearchableArtificialIntelligence> entityService,
            ILogger<ArtificialIntelligenceController> logger) : base(entityService, logger)
        {
        }

        public Task<IEnumerable<ArtificialIntelligence>> InvokeGetEntitiesByComplexQuery(
            ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
