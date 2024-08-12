using Newsy.Core.Models;
using Newsy.Persistence.Models;

namespace Newsy.Core.Contracts.Services;

public interface IAuthorService
{
    Task<Author?> GetAuthorAsync(Guid id);
    Task<Author[]> GetAuthorsAsync(int pageNumber, int pageSize);
    Task<bool> UpdateAuthorAsync(Guid id, UpsertAuthorServiceModel upsertAuthorServiceModel);
}
