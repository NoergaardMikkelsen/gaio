using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Statistics.Api.Controllers;
using Statistics.Shared.Abstraction.Interfaces.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Tests.Controllers
{
    [TestFixture]
    public class KeywordControllerTests
    {
        private Mock<IEntityQueryService<Keyword, SearchableKeyword>> _mockEntityService;
        private Mock<ILogger<KeywordController>> _mockLogger;
        private KeywordController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockEntityService = new Mock<IEntityQueryService<Keyword, SearchableKeyword>>();
            _mockLogger = new Mock<ILogger<KeywordController>>();
            _controller = new KeywordController(_mockEntityService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResultWithEntities()
        {
            // Arrange
            var expectedEntities = new List<Keyword>
            {
                new Keyword(1) { Text = "Keyword1" },
                new Keyword(2) { Text = "Keyword2" }
            };
            _mockEntityService
                .Setup(service => service.GetEntities(It.IsAny<SearchableKeyword>()))
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
            var expectedEntity = new Keyword(id) { Text = "Keyword1" };
            _mockEntityService
                .Setup(service => service.GetEntity(It.Is<SearchableKeyword>(s => s.Id == id)))
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
            var searchable = new SearchableKeyword { Text = "TestKeyword" };
            var expectedEntity = new Keyword(1) { Text = "Keyword1" };
            _mockEntityService
                .Setup(service => service.GetEntity(searchable))
                .ReturnsAsync(expectedEntity);

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
            var entity = new Keyword(1) { Text = "Keyword1" };

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
            var entity = new Keyword(1) { Text = "UpdatedKeyword" };

            // Act
            var result = await _controller.UpdateSingle(entity) as OkResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            _mockEntityService.Verify(service => service.UpdateEntity(entity, It.IsAny<bool>()), Times.Once);
        }
    }
}
