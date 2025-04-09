using Microsoft.EntityFrameworkCore;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;
using Statistics.Shared.Persistence;
using Statistics.Shared.Persistence.Services;

namespace Statistics.Tests.Service;

[TestFixture]
public class PromptQueryServiceTests
{
    private StatisticsDatabaseContext _context;
    private TestablePromptQueryService _service;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StatisticsDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StatisticsDatabaseContext(options);

        // Seed data
        _context.Prompts.AddRange(
            new Prompt() { Text = "Prompt 1" },
            new Prompt() { Text = "Prompt 2" }
        );
        _context.SaveChanges();

        // Initialize testable service
        _service = new TestablePromptQueryService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void GetBaseQuery_ShouldReturnAllPromptsWithResponsesAndAi()
    {
        // Act
        var result = _service.TestGetBaseQuery().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Text.Should().Be("Prompt 1");
        result[1].Text.Should().Be("Prompt 2");
    }

    [Test]
    public void AddQueryArguments_ShouldFilterByText()
    {
        // Arrange
        var searchable = new SearchablePrompt { Text = "Prompt 1" };
        var baseQuery = _service.TestGetBaseQuery();

        // Act
        var result = _service.TestAddQueryArguments(searchable, baseQuery).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Text.Should().Be("Prompt 1");
    }

    [Test]
    public async Task GetEntity_ShouldReturnMatchingPrompt()
    {
        // Arrange
        var searchable = new SearchablePrompt { Text = "Prompt 1" };

        // Act
        var result = await _service.GetEntity(searchable);

        // Assert
        result.Should().NotBeNull();
        result.Text.Should().Be("Prompt 1");
    }

    [Test]
    public async Task GetEntities_ShouldReturnAllMatchingPrompts()
    {
        // Arrange
        var searchable = new SearchablePrompt { Text = "Prompt" };

        // Act
        var result = await _service.GetEntities(searchable);

        // Assert
        result.Should().HaveCount(2);
        result.First().Text.Should().Be("Prompt 1");
        result.Last().Text.Should().Be("Prompt 2");
    }

    [Test]
    public async Task AddEntity_ShouldAddPromptToDbSet()
    {
        // Arrange
        var newPrompt = new Prompt(3) { Text = "Prompt 3" };

        // Act
        await _service.AddEntity(newPrompt);

        // Assert
        var addedPrompt = await _context.Prompts.FindAsync(3);
        addedPrompt.Should().NotBeNull();
        addedPrompt.Text.Should().Be("Prompt 3");
    }

    [Test]
    public async Task UpdateEntity_ShouldUpdatePromptInDbSet()
    {
        // Arrange
        var updatedPrompt = await _context.Prompts.FindAsync(1);
        updatedPrompt.Text = "Updated Prompt 1";

        // Act
        await _service.UpdateEntity(updatedPrompt);

        // Assert
        var prompt = await _context.Prompts.FindAsync(1);
        prompt.Should().NotBeNull();
        prompt.Text.Should().Be("Updated Prompt 1");
    }

    [Test]
    public async Task DeleteEntity_ShouldRemovePromptFromDbSet()
    {
        // Arrange
        var searchable = new SearchablePrompt { Text = "Prompt 1" };

        // Act
        await _service.DeleteEntity(searchable);

        // Assert
        var prompt = _context.Prompts.FirstOrDefault(p => p.Text == "Prompt 1");
        prompt.Should().BeNull();
    }

    [Test]
    public async Task DeleteEntityById_ShouldRemovePromptById()
    {
        // Arrange
        var id = 1;

        // Act
        await _service.DeleteEntityById(id);

        // Assert
        var prompt = await _context.Prompts.FindAsync(id);
        prompt.Should().BeNull();
    }

    private class TestablePromptQueryService : PromptQueryService
    {
        public TestablePromptQueryService(StatisticsDatabaseContext context) : base(context)
        {
        }

        public IQueryable<Prompt> TestGetBaseQuery()
        {
            return GetBaseQuery();
        }

        public IQueryable<Prompt> TestAddQueryArguments(SearchablePrompt searchable, IQueryable<Prompt> query)
        {
            return AddQueryArguments(searchable, query);
        }
    }
}
