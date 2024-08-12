using Newsy.Abstractions.Models;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.Core.Contracts.Services;

public interface IArticleService
{
    Task<Article?> GetArticleByIdAsync(Guid id);
    Task<BasicArticleModel[]> GetArticlesAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters);
    Task<bool> DeleteArticleAsync(Guid id);
    Task<Guid> CreateArticleAsync(UpsertArticleServiceModel upsertArticleServiceModel);
    Task<bool> UpdateArticleAsync(Guid id, UpsertArticleServiceModel upsertArticleServiceModel);
}
