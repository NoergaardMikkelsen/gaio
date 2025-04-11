using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Statistics.Api.Controllers;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Tests.Controllers;

[TestFixture]
public class KeywordControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _mockEntityService = new Mock<IEntityQueryService<Keyword, SearchableKeyword>>();
        _mockLogger = new Mock<ILogger<KeywordController>>();
        _controller = new TestableKeywordController(_mockEntityService.Object, _mockLogger.Object);
    }

    private Mock<IEntityQueryService<Keyword, SearchableKeyword>> _mockEntityService;
    private Mock<ILogger<KeywordController>> _mockLogger;
    private TestableKeywordController _controller;

    [Test]
    public async Task GetAll_ReturnsOkResultWithEntities()
    {
        // Arrange
        var expectedEntities = new List<Keyword>
        {
            new(1) {Text = "Keyword1",},
            new(2) {Text = "Keyword2",},
        };
        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableKeyword>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAll() as OkObjectResult;

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
        var expectedEntity = new Keyword(id) {Text = "Keyword1",};
        _mockEntityService.Setup(service => service.GetEntity(It.Is<SearchableKeyword>(s => s.Id == id)))
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
        var searchable = new SearchableKeyword {Text = "TestKeyword",};
        var expectedEntity = new Keyword(1) {Text = "Keyword1",};
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
        var entity = new Keyword(1) {Text = "Keyword1",};

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
    public async Task UpdateSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new Keyword(1) {Text = "UpdatedKeyword",};

        // Act
        var result = await _controller.UpdateSingle(entity) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntity(entity, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task AddMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<Keyword>
        {
            new(1) {Text = "Keyword1",},
            new(2) {Text = "Keyword2",},
        };

        // Act
        var result = await _controller.AddMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task UpdateMultiple_ReturnsOkResult()
    {
        // Arrange
        var entities = new List<Keyword>
        {
            new(1) {Text = "UpdatedKeyword1",},
            new(2) {Text = "UpdatedKeyword2",},
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
            SearchableKeyword = new SearchableKeyword {Text = "TestKeyword",},
        };
        var expectedEntities = new List<Keyword>
        {
            new(1) {Text = "Keyword1",},
            new(2) {Text = "Keyword2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableKeyword>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAllByComplexQuery(complexSearchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task GetAllByQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var searchable = new SearchableKeyword {Text = "TestKeyword",};
        var expectedEntities = new List<Keyword>
        {
            new(1) {Text = "Keyword1",},
            new(2) {Text = "Keyword2",},
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
        var searchable = new SearchableKeyword {Text = "TestKeyword",};

        // Act
        var result = await _controller.DeleteByQuery(searchable) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.DeleteEntity(searchable, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetEntitiesByComplexQuery_WithValidSearchableKeyword_ReturnsEntities()
    {
        // Arrange
        var searchableKeyword = new SearchableKeyword {Text = "TestKeyword",};
        var complexSearchable = new ComplexSearchable {SearchableKeyword = searchableKeyword,};
        var expectedEntities = new List<Keyword>
        {
            new(1) {Text = "Keyword1",},
            new(2) {Text = "Keyword2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(searchableKeyword)).ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public void GetEntitiesByComplexQuery_WithNullSearchableKeyword_ThrowsArgumentNullException()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable {SearchableKeyword = null,};

        // Act
        Func<Task> act = async () => await _controller.GetAllByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchableKeyword')");
    }

    private class TestableKeywordController : KeywordController
    {
        public TestableKeywordController(
            IEntityQueryService<Keyword, SearchableKeyword> entityService,
            ILogger<KeywordController> logger) : base(entityService, logger)
        {
        }

        public Task<IEnumerable<Keyword>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
