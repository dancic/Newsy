using FluentResults;
using Newsy.Abstractions.Models;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.Core.Contracts.Services;

public interface IAuthorService
{
    Task<Result<Author?>> GetAuthorAsync(Guid id);
    Task<Result<BasicAuthorModel[]>> GetAuthorsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters);
    Task<Result> UpdateAuthorAsync(Guid id, UpsertAuthorServiceModel upsertAuthorServiceModel);
}
