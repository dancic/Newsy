using FluentAssertions;
using Moq;
using Newsy.Abstractions.Models;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Core.Services;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Exceptions;
using Newsy.Persistence.Models;

namespace Newsy.UnitTests;

public class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> articleRepositoryMock;
    private readonly Mock<IAuthorRepository> authorRepositoryMock;
    private readonly Mock<IAuthService> authServiceMock;
    private readonly ArticleService articleService;

    public ArticleServiceTests()
    {
        articleRepositoryMock = new Mock<IArticleRepository>();
        authorRepositoryMock = new Mock<IAuthorRepository>();
        authServiceMock = new Mock<IAuthService>();
        articleService = new ArticleService(articleRepositoryMock.Object, authorRepositoryMock.Object, authServiceMock.Object);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExists()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var article = new Article { Id = articleId, Author = new Author { ApplicationUserId = userId } };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync(article);

        // Act
        var result = await articleService.GetArticleByIdAsync(articleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(article);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldReturnNull_WhenArticleDoesNotExist()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await articleService.GetArticleByIdAsync(articleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetArticlesAsync_ShouldReturnArticles_WhenNoExceptions()
    {
        // Arrange
        var articles = new BasicArticleModel[]
        {
            new BasicArticleModel { Id = Guid.NewGuid(), Title = "Article 1" },
            new BasicArticleModel { Id = Guid.NewGuid(), Title = "Article 2" }
        };
        var userId = Guid.NewGuid().ToString();

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetArticleGridDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<Filter>>(), userId))
            .ReturnsAsync(articles);

        // Act
        var result = await articleService.GetArticlesAsync(1, 10, new List<Filter>());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(articles);
    }

    [Fact]
    public async Task GetArticlesAsync_ShouldReturnError_WhenInvalidFilterExceptionOccurs()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var ex = new InvalidFilterException("Invalid filter");

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetArticleGridDataAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IEnumerable<Filter>>(), userId))
            .ThrowsAsync(ex);

        // Act
        var result = await articleService.GetArticlesAsync(1, 10, new List<Filter>());

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid filter");
    }

    [Fact]
    public async Task CreateArticleAsync_ShouldReturnArticleId_WhenArticleIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var author = new Author { Id = Guid.NewGuid(), ApplicationUser = new ApplicationUser { FullName = "Author Name" } };
        var upsertModel = new UpsertArticleServiceModel { Title = "New Article", Content = "Content", IsPublished = true };
        var newArticleId = Guid.NewGuid();

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        authorRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(author);
        articleRepositoryMock.Setup(r => r.AddArticleAsync(It.IsAny<Article>(), author.ApplicationUser.FullName))
            .ReturnsAsync(newArticleId);

        // Act
        var result = await articleService.CreateArticleAsync(upsertModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(newArticleId);
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldReturnOk_WhenArticleIsUpdated()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var article = new Article { Id = articleId, Author = new Author { ApplicationUserId = userId, ApplicationUser = new ApplicationUser() { FullName = "User" } } };
        var updateModel = new UpsertArticleServiceModel { Title = "Updated Title", Content = "Updated Content", IsPublished = true };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync(article);
        articleRepositoryMock.Setup(r => r.UpdateArticleAsync(article, article.Author.ApplicationUser.FullName))
            .Returns(Task.CompletedTask);

        // Act
        var result = await articleService.UpdateArticleAsync(articleId, updateModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldReturnFailure_WhenArticleDoesNotExist()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var updateModel = new UpsertArticleServiceModel { Title = "Updated Title", Content = "Updated Content", IsPublished = true };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await articleService.UpdateArticleAsync(articleId, updateModel);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldReturnFailure_WhenUserIsNotAuthor()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var differentUserId = Guid.NewGuid().ToString();
        var article = new Article { Id = articleId, Author = new Author { ApplicationUserId = differentUserId } };
        var updateModel = new UpsertArticleServiceModel { Title = "Updated Title", Content = "Updated Content", IsPublished = true };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync(article);

        // Act
        var result = await articleService.UpdateArticleAsync(articleId, updateModel);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldReturnOk_WhenArticleIsDeleted()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var article = new Article { Id = articleId, Author = new Author { ApplicationUserId = userId, ApplicationUser = new ApplicationUser { FullName = "User" } } };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync(article);
        articleRepositoryMock.Setup(r => r.SoftDeleteArticleAsync(article, article.Author.ApplicationUser.FullName))
            .ReturnsAsync(true);

        // Act
        var result = await articleService.DeleteArticleAsync(articleId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldReturnFailure_WhenArticleDoesNotExist()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await articleService.DeleteArticleAsync(articleId);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldReturnFailure_WhenUserIsNotAuthor()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var differentUserId = Guid.NewGuid().ToString();
        var article = new Article { Id = articleId, Author = new Author { ApplicationUserId = differentUserId } };

        authServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId, userId))
            .ReturnsAsync(article);

        // Act
        var result = await articleService.DeleteArticleAsync(articleId);

        // Assert
        result.IsFailed.Should().BeTrue();
    }
}