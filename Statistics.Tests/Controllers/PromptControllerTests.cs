using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Statistics.Api.Controllers;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Tests.Controllers;

[TestFixture]
public class PromptControllerTests
{
    private Mock<IEntityQueryService<Prompt, SearchablePrompt>> _mockEntityService;
    private Mock<ILogger<PromptController>> _mockLogger;
    private TestablePromptController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockEntityService = new Mock<IEntityQueryService<Prompt, SearchablePrompt>>();
        _mockLogger = new Mock<ILogger<PromptController>>();
        _controller = new TestablePromptController(_mockEntityService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResultWithEntities()
    {
        // Arrange
        var expectedEntities = new List<Prompt>
        {
            new(1) {Text = "Prompt1",},
            new(2) {Text = "Prompt2",},
        };
        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchablePrompt>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAll() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task GetEntitiesByComplexQuery_WithValidSearchablePrompt_ReturnsEntities()
    {
        // Arrange
        var searchablePrompt = new SearchablePrompt {Text = "TestPrompt",};
        var complexSearchable = new ComplexSearchable {SearchablePrompt = searchablePrompt,};
        var expectedEntities = new List<Prompt>
        {
            new(1) {Text = "Prompt1",},
            new(2) {Text = "Prompt2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(searchablePrompt)).ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public void GetEntitiesByComplexQuery_WithNullSearchablePrompt_ThrowsArgumentNullException()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable {SearchablePrompt = null,};

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchablePrompt')");
    }

    [Test]
    public async Task GetAllByQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var searchable = new SearchablePrompt {Text = "TestPrompt",};
        var expectedEntities = new List<Prompt>
        {
            new(1) {Text = "Prompt1",},
            new(2) {Text = "Prompt2",},
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
    public async Task DeleteByQuery_ReturnsOkResult()
    {
        // Arrange
        var searchable = new SearchablePrompt {Text = "TestPrompt",};

        // Act
        var result = await _controller.DeleteByQuery(searchable) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.DeleteEntity(searchable, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task AddMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<Prompt>
        {
            new(1) {Text = "Prompt1",},
            new(2) {Text = "Prompt2",},
        };

        // Act
        var result = await _controller.AddMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntities(entities, It.IsAny<bool>()), Times.Once);
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
    public async Task GetById_ReturnsOkResultWithEntity()
    {
        // Arrange
        var id = 1;
        var expectedEntity = new Prompt(id) {Text = "Prompt1",};
        _mockEntityService.Setup(service => service.GetEntity(It.Is<SearchablePrompt>(s => s.Id == id)))
            .ReturnsAsync(expectedEntity);

        // Act
        var result = await _controller.GetById(id) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntity);
    }

    [Test]
    public async Task GetByQuery_ReturnsOkResultWithEntity()
    {
        // Arrange
        var searchable = new SearchablePrompt {Text = "TestPrompt",};
        var expectedEntity = new Prompt(1) {Text = "Prompt1",};
        _mockEntityService.Setup(service => service.GetEntity(searchable)).ReturnsAsync(expectedEntity);

        // Act
        var result = await _controller.GetByQuery(searchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntity);
    }

    [Test]
    public async Task AddSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new Prompt(1) {Text = "Prompt1",};

        // Act
        var result = await _controller.AddSingle(entity) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntity(entity, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task UpdateSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new Prompt(1) {Text = "UpdatedPrompt",};

        // Act
        var result = await _controller.UpdateSingle(entity) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntity(entity, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task UpdateMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<Prompt>
        {
            new(1) {Text = "UpdatedPrompt1",},
            new(2) {Text = "UpdatedPrompt2",},
        };

        // Act
        var result = await _controller.UpdateMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetAllByComplexQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable
        {
            SearchablePrompt = new SearchablePrompt {Text = "TestPrompt",},
        };
        var expectedEntities = new List<Prompt>
        {
            new(1) {Text = "Prompt1",},
            new(2) {Text = "Prompt2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchablePrompt>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAllByComplexQuery(complexSearchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }


    private class TestablePromptController : PromptController
    {
        public TestablePromptController(
            IEntityQueryService<Prompt, SearchablePrompt> entityService, ILogger<PromptController> logger) : base(
            entityService, logger)
        {
        }

        public Task<IEnumerable<Prompt>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
