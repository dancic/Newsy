using FluentAssertions;
using Moq;
using Newsy.Abstractions.Models;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Core.Services;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Models;

namespace Newsy.UnitTests;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> authorRepositoryMock;
    private readonly Mock<IAuthService> authServiceMock;
    private readonly IAuthorService authorService;

    public AuthorServiceTests()
    {
        authorRepositoryMock = new Mock<IAuthorRepository>();
        authServiceMock = new Mock<IAuthService>();
        authorService = new AuthorService(authorRepositoryMock.Object, authServiceMock.Object);
    }

    [Fact]
    public async Task GetAuthorAsync_ShouldReturnAuthor_WhenAuthorExists()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var author = new Author { Id = authorId, ApplicationUserId = userId };
        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByIdAsync(authorId, userId))
            .ReturnsAsync(author);

        // Act
        var result = await authorService.GetAuthorAsync(authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(author);
    }

    [Fact]
    public async Task GetAuthorAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByIdAsync(authorId, userId))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await authorService.GetAuthorAsync(authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetAuthorsAsync_ShouldReturnAuthors()
    {
        // Arrange
        var authors = new BasicAuthorModel[]
        {
            new BasicAuthorModel { Id = Guid.NewGuid(), Name = "Author 1" },
            new BasicAuthorModel { Id = Guid.NewGuid(), Name = "Author 2" }
        };
        authorRepositoryMock.Setup(r => r.GetAuthorsGridDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<Filter>>()))
            .ReturnsAsync(authors);

        // Act
        var result = await authorService.GetAuthorsAsync(1, 10, new List<Filter>());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(authors);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldReturnOk_WhenAuthorIsUpdated()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var author = new Author { Id = authorId, ApplicationUserId = userId, ApplicationUser = new ApplicationUser { FullName = "Old Name" } };
        var updateModel = new UpsertAuthorServiceModel { Name = "New Name", Bio = "New Bio" };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByIdAsync(authorId, userId))
            .ReturnsAsync(author);
        authorRepositoryMock.Setup(r => r.UpdateUserAsync(author))
            .Returns(Task.CompletedTask);

        // Act
        var result = await authorService.UpdateAuthorAsync(authorId, updateModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        authorRepositoryMock.Verify(r => r.UpdateUserAsync(author), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldReturnNotFound_WhenAuthorDoesNotExist()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var updateModel = new UpsertAuthorServiceModel { Name = "New Name", Bio = "New Bio" };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByIdAsync(authorId, userId))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await authorService.UpdateAuthorAsync(authorId, updateModel);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldReturnForbidden_WhenAuthorIsNotCurrentUser()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var differentUserId = Guid.NewGuid().ToString();
        var author = new Author { Id = authorId, ApplicationUserId = differentUserId };
        var updateModel = new UpsertAuthorServiceModel { Name = "New Name", Bio = "New Bio" };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByIdAsync(authorId, userId))
            .ReturnsAsync(author);

        // Act
        var result = await authorService.UpdateAuthorAsync(authorId, updateModel);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

}
