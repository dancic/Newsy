using Newsy.Abstractions.Models;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Persistence.Contracts.Services;
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

    public async Task<bool> DeleteArticleAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        var articleToDelete = await articleRepository.GetByIdAsync(id, currentUserId);

        if (articleToDelete == null || articleToDelete.Author.ApplicationUserId != currentUserId)
        {
            return false;
        }

        return await articleRepository.SoftDeleteArticleAsync(articleToDelete, articleToDelete.Author.ApplicationUser.FullName);
    }

    public async Task<Article?> GetArticleByIdAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        return await articleRepository.GetByIdAsync(id, currentUserId);
    }

    public async Task<BasicArticleModel[]> GetArticlesAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters)
    {
        var currentUserId = authService.GetCurrentUserId();
        return await articleRepository.GetArticleGridDataAsync(pageNumber, pageSize, filters, currentUserId);
    }

    public async Task<Guid> CreateArticleAsync(UpsertArticleServiceModel upsertArticleServiceModel)
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

        return await articleRepository.AddArticleAsync(article, author.ApplicationUser.FullName);
    }

    public async Task<bool> UpdateArticleAsync(Guid id, UpsertArticleServiceModel upsertArticleServiceModel)
    {
        var currentUserId = authService.GetCurrentUserId();
        var articleToUpdate = await articleRepository.GetByIdAsync(id, currentUserId);

        if (articleToUpdate == null || articleToUpdate.Author.ApplicationUserId != currentUserId)
        {
            return false;
        }

        articleToUpdate.Title = upsertArticleServiceModel.Title;
        articleToUpdate.Content = upsertArticleServiceModel.Content;
        articleToUpdate.LastPublishedDateTime = articleToUpdate.IsPublished == false && upsertArticleServiceModel.IsPublished == true ? DateTimeOffset.UtcNow : articleToUpdate.LastPublishedDateTime;
        articleToUpdate.IsPublished = upsertArticleServiceModel.IsPublished;

        await articleRepository.UpdateArticleAsync(articleToUpdate, articleToUpdate.Author.ApplicationUser.FullName);

        return true;
    }
}
