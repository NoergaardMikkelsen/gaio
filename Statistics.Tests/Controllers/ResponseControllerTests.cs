using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Statistics.Api.Controllers;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Tests.Controllers;

[TestFixture]
public class ResponseControllerTests
{
    private Mock<IEntityQueryService<Response, SearchableResponse>> _mockEntityService;
    private Mock<ILogger<ResponseController>> _mockLogger;
    private TestableResponseController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockEntityService = new Mock<IEntityQueryService<Response, SearchableResponse>>();
        _mockLogger = new Mock<ILogger<ResponseController>>();
        _controller = new TestableResponseController(_mockEntityService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkResultWithEntities()
    {
        // Arrange
        var expectedEntities = new List<Response>
        {
            new(1) {Text = "Response1",},
            new(2) {Text = "Response2",},
        };
        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableResponse>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAll() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task GetAllByComplexQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable
        {
            SearchableResponse = new SearchableResponse {Text = "TestResponse",},
        };
        var expectedEntities = new List<Response>
        {
            new(1) {Text = "Response1",},
            new(2) {Text = "Response2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(It.IsAny<SearchableResponse>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAllByComplexQuery(complexSearchable) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }


    [Test]
    public async Task GetEntitiesByComplexQuery_WithValidSearchableResponse_ReturnsEntities()
    {
        // Arrange
        var searchableResponse = new SearchableResponse {Text = "TestResponse",};
        var complexSearchable = new ComplexSearchable {SearchableResponse = searchableResponse,};
        var expectedEntities = new List<Response>
        {
            new(1) {Text = "Response1",},
            new(2) {Text = "Response2",},
        };

        _mockEntityService.Setup(service => service.GetEntities(searchableResponse)).ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public void GetEntitiesByComplexQuery_WithNullSearchableResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var complexSearchable = new ComplexSearchable {SearchableResponse = null,};

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchableResponse')");
    }

    [Test]
    public async Task UpdateSingle_ReturnsOkResult()
    {
        // Arrange
        var entity = new Response(1) {Text = "UpdatedResponse",};

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
        var entities = new List<Response>
        {
            new(1) {Text = "Response1",},
            new(2) {Text = "Response2",},
        };

        // Act
        var result = await _controller.AddMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.AddEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task DeleteByQuery_ReturnsOkResult()
    {
        // Arrange
        var searchable = new SearchableResponse {Text = "TestResponse",};

        // Act
        var result = await _controller.DeleteByQuery(searchable) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.DeleteEntity(searchable, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetAllByQuery_ReturnsOkResultWithEntities()
    {
        // Arrange
        var searchable = new SearchableResponse {Text = "TestResponse",};
        var expectedEntities = new List<Response>
        {
            new(1) {Text = "Response1",},
            new(2) {Text = "Response2",},
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
        var entities = new List<Response>
        {
            new(1) {Text = "UpdatedResponse1",},
            new(2) {Text = "UpdatedResponse2",},
        };

        // Act
        var result = await _controller.UpdateMultiple(entities) as OkResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        _mockEntityService.Verify(service => service.UpdateEntities(entities, It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetById_ReturnsOkResultWithEntity()
    {
        // Arrange
        var id = 1;
        var expectedEntity = new Response(id) {Text = "Response1",};
        _mockEntityService.Setup(service => service.GetEntity(It.Is<SearchableResponse>(s => s.Id == id)))
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
        var searchable = new SearchableResponse {Text = "TestResponse",};
        var expectedEntity = new Response(1) {Text = "Response1",};
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
        var entity = new Response(1) {Text = "Response1",};

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


    private class TestableResponseController : ResponseController
    {
        public TestableResponseController(
            IEntityQueryService<Response, SearchableResponse> entityService,
            ILogger<ResponseController> logger) : base(entityService, logger)
        {
        }

        public Task<IEnumerable<Response>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
