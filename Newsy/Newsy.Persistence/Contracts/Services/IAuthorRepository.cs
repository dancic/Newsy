using Newsy.Abstractions.Models;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Contracts.Services;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(Guid id, string currentUserId);
    Task<Author?> GetByUserIdAsync(string authorUserId);
    Task<BasicAuthorModel[]> GetAuthorsGridDataAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters);
    Task CreateAuthorAsync(Author author);
    Task UpdateUserAsync(Author authorToUpdate);
}
