using FluentResults;
using Newsy.Abstractions.Models;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.Core.Contracts.Services;

public interface IArticleService
{
    Task<Result<Article?>> GetArticleByIdAsync(Guid id);
    Task<Result<BasicArticleModel[]>> GetArticlesAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters);
    Task<Result> DeleteArticleAsync(Guid id);
    Task<Result<Guid>> CreateArticleAsync(UpsertArticleServiceModel upsertArticleServiceModel);
    Task<Result> UpdateArticleAsync(Guid id, UpsertArticleServiceModel upsertArticleServiceModel);
}
