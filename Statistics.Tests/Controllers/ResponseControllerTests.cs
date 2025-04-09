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
            new Response(1) { Text = "Response1" },
            new Response(2) { Text = "Response2" }
        };
        _mockEntityService
            .Setup(service => service.GetEntities(It.IsAny<SearchableResponse>()))
            .ReturnsAsync(expectedEntities);

        // Act
        var result = await _controller.GetAll() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(expectedEntities);
    }

    [Test]
    public async Task GetEntitiesByComplexQuery_WithValidSearchableResponse_ReturnsEntities()
    {
        // Arrange
        var searchableResponse = new SearchableResponse { Text = "TestResponse" };
        var complexSearchable = new ComplexSearchable { SearchableResponse = searchableResponse };
        var expectedEntities = new List<Response>
        {
            new Response(1) { Text = "Response1" },
            new Response(2) { Text = "Response2" }
        };

        _mockEntityService
            .Setup(service => service.GetEntities(searchableResponse))
            .ReturnsAsync(expectedEntities);

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
        var complexSearchable = new ComplexSearchable { SearchableResponse = null };

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchableResponse')");
    }

    // Private wrapper class to expose the protected method
    private class TestableResponseController : ResponseController
    {
        public TestableResponseController(
            IEntityQueryService<Response, SearchableResponse> entityService,
            ILogger<ResponseController> logger)
            : base(entityService, logger)
        {
        }

        public Task<IEnumerable<Response>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
