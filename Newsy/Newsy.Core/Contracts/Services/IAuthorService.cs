using Newsy.Abstractions.Models;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.Core.Contracts.Services;

public interface IAuthorService
{
    Task<Author?> GetAuthorAsync(Guid id);
    Task<BasicAuthorModel[]> GetAuthorsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters);
    Task<bool> UpdateAuthorAsync(Guid id, UpsertAuthorServiceModel upsertAuthorServiceModel);
}
