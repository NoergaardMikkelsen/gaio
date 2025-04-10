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
            new Prompt(1) { Text = "Prompt1" },
            new Prompt(2) { Text = "Prompt2" }
        };
        _mockEntityService
            .Setup(service => service.GetEntities(It.IsAny<SearchablePrompt>()))
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
        var searchablePrompt = new SearchablePrompt { Text = "TestPrompt" };
        var complexSearchable = new ComplexSearchable { SearchablePrompt = searchablePrompt };
        var expectedEntities = new List<Prompt>
        {
            new Prompt(1) { Text = "Prompt1" },
            new Prompt(2) { Text = "Prompt2" }
        };

        _mockEntityService
            .Setup(service => service.GetEntities(searchablePrompt))
            .ReturnsAsync(expectedEntities);

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
        var complexSearchable = new ComplexSearchable { SearchablePrompt = null };

        // Act
        Func<Task> act = async () => await _controller.InvokeGetEntitiesByComplexQuery(complexSearchable);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'SearchablePrompt')");
    }

    // Private wrapper class to expose the protected method
    private class TestablePromptController : PromptController
    {
        public TestablePromptController(
            IEntityQueryService<Prompt, SearchablePrompt> entityService,
            ILogger<PromptController> logger)
            : base(entityService, logger)
        {
        }

        public Task<IEnumerable<Prompt>> InvokeGetEntitiesByComplexQuery(ComplexSearchable complexSearchable)
        {
            return GetEntitiesByComplexQuery(complexSearchable);
        }
    }
}
