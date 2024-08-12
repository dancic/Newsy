using Microsoft.EntityFrameworkCore;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Extensions;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Services;

public class AuthorRepository : IAuthorRepository
{
    private readonly AppDbContext context;

    public AuthorRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Author[]> GetAuthorsGridDataAsync(int pageNumber, int pageSize)
    {
        return await context.Authors
            .Include(a => a.ApplicationUser)
            .Where(a => a.DeletedAt == null)
            .Paginate(pageNumber, pageSize)
            .ToArrayAsync();
    }

    public async Task<Author?> GetByIdAsync(Guid id, string currentUserId)
    {
        return await context.Authors
            .Include(a => a.ApplicationUser)
            .Include(a => a.Articles.Where(a => a.IsPublished || a.Author.ApplicationUserId == currentUserId))
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);
    }

    public async Task<Author?> GetByUserIdAsync(string authorUserId)
    {
        return await context.Authors
            .Include(a => a.ApplicationUser)
            .FirstOrDefaultAsync(a => a.ApplicationUserId == authorUserId && a.DeletedAt == null);
    }

    public async Task CreateAuthorAsync(Author author)
    {
        await context.Authors.AddAsync(author);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(Author authorToUpdate)
    {
        context.Authors.Update(authorToUpdate);
        await context.SaveChangesAsync();
    }
}
