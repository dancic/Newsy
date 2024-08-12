using Newsy.Persistence.Models;

namespace Newsy.Persistence.Contracts.Services;

public interface IArticleRepository
{
    Task<Guid> AddArticleAsync(Article article, string addedBy);
    Task<bool> SoftDeleteArticleAsync(Article article, string deletedBy);
    Task<Article[]> GetArticleGridDataAsync(int pageNumber, int pageSize, string requesterUserId);
    Task<Article?> GetByIdAsync(Guid id, string requesterUserId);
    Task UpdateArticleAsync(Article article, string updatedBy);
}
