using FluentResults;
using Newsy.Abstractions;
using Newsy.Abstractions.Models;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Exceptions;
using Newsy.Persistence.Models;

namespace Newsy.Core.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository articleRepository;
    private readonly IAuthorRepository authorRepository;
    private readonly IAuthService authService;

    public ArticleService(IArticleRepository articleRepository,
        IAuthorRepository authorRepository,
        IAuthService authService)
    {
        this.articleRepository = articleRepository;
        this.authorRepository = authorRepository;
        this.authService = authService;
    }

    public async Task<Result<Article?>> GetArticleByIdAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        return Result.Ok(await articleRepository.GetByIdAsync(id, currentUserId));
    }

    public async Task<Result<BasicArticleModel[]>> GetArticlesAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters)
    {
        var currentUserId = authService.GetCurrentUserId();
        try
        {
            var articles = await articleRepository.GetArticleGridDataAsync(pageNumber, pageSize, filters, currentUserId);
            return Result.Ok(articles);
        }
        catch (InvalidFilterException ex)
        {
            return Result.Fail(new Error(ex.Message).WithMetadata(Constants.StatusCodeMetadataName, 400));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(ex.Message).WithMetadata(Constants.StatusCodeMetadataName, 500));
        }
    }

    public async Task<Result<Guid>> CreateArticleAsync(UpsertArticleServiceModel upsertArticleServiceModel)
    {
        var currentUserId = authService.GetCurrentUserId();
        var author = await authorRepository.GetByUserIdAsync(currentUserId);

        var article = new Article()
        {
            Title = upsertArticleServiceModel.Title,
            Content = upsertArticleServiceModel.Content,
            AuthorId = author.Id,
            IsPublished = upsertArticleServiceModel.IsPublished,
            LastPublishedDateTime = upsertArticleServiceModel.IsPublished ? DateTime.UtcNow : null
        };

        return Result.Ok(await articleRepository.AddArticleAsync(article, author.ApplicationUser.FullName));
    }

    public async Task<Result> UpdateArticleAsync(Guid id, UpsertArticleServiceModel upsertArticleServiceModel)
    {
        var currentUserId = authService.GetCurrentUserId();
        var articleToUpdate = await articleRepository.GetByIdAsync(id, currentUserId);

        if (articleToUpdate == null)
        {
            return Result.Fail(new Error("There is no article with given id.").WithMetadata(Constants.StatusCodeMetadataName, 404));
        }

        if (articleToUpdate.Author.ApplicationUserId != currentUserId)
        {
            return Result.Fail(new Error("Only author can update article.").WithMetadata(Constants.StatusCodeMetadataName, 403));
        }

        articleToUpdate.Title = upsertArticleServiceModel.Title;
        articleToUpdate.Content = upsertArticleServiceModel.Content;
        articleToUpdate.LastPublishedDateTime = articleToUpdate.IsPublished == false && upsertArticleServiceModel.IsPublished == true ? DateTimeOffset.UtcNow : articleToUpdate.LastPublishedDateTime;
        articleToUpdate.IsPublished = upsertArticleServiceModel.IsPublished;

        await articleRepository.UpdateArticleAsync(articleToUpdate, articleToUpdate.Author.ApplicationUser.FullName);
        return Result.Ok();
    }

    public async Task<Result> DeleteArticleAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        var articleToDelete = await articleRepository.GetByIdAsync(id, currentUserId);

        if (articleToDelete == null)
        {
            return Result.Fail(new Error("There is no article with given id.").WithMetadata(Constants.StatusCodeMetadataName, 404));
        }

        if (articleToDelete.Author.ApplicationUserId != currentUserId)
        {
            return Result.Fail(new Error("Only author can update article.").WithMetadata(Constants.StatusCodeMetadataName, 403));
        }

        await articleRepository.SoftDeleteArticleAsync(articleToDelete, articleToDelete.Author.ApplicationUser.FullName);
        return Result.Ok();
    }
}
