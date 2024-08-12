using Newsy.Persistence.Models;

namespace Newsy.Persistence.Contracts.Services;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(Guid id, string currentUserId);
    Task<Author?> GetByUserIdAsync(string authorUserId);
    Task<Author[]> GetAuthorsGridDataAsync(int pageNumber, int pageSize);
    Task CreateAuthorAsync(Author author);
    Task UpdateUserAsync(Author authorToUpdate);
}
