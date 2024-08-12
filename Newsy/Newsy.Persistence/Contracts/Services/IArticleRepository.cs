using Newsy.Abstractions.Models;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Contracts.Services;

public interface IArticleRepository
{
    Task<Guid> AddArticleAsync(Article article, string addedBy);
    Task<bool> SoftDeleteArticleAsync(Article article, string deletedBy);
    Task<BasicArticleModel[]> GetArticleGridDataAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, string requesterUserId);
    Task<Article?> GetByIdAsync(Guid id, string requesterUserId);
    Task UpdateArticleAsync(Article article, string updatedBy);
}
