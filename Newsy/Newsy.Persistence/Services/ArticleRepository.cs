using Microsoft.EntityFrameworkCore;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Extensions;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Services;

public class ArticleRepository : IArticleRepository
{
    private readonly AppDbContext context;

    public ArticleRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Guid> AddArticleAsync(Article article, string addedBy)
    {
        article.PrepareUpdate(addedBy);
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();
        return article.Id;
    }

    public async Task<bool> SoftDeleteArticleAsync(Article article, string deletedBy)
    {
        article.PrepareDelete(deletedBy);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Article[]> GetArticleGridDataAsync(int pageNumber, int pageSize, string requesterUserId)
    {
        return await context.Articles
            .Include(a => a.Author)
                .ThenInclude(a => a.ApplicationUser)
            .Where(a => a.DeletedAt == null && (a.Author.ApplicationUserId == requesterUserId || a.IsPublished))
            .Paginate(pageNumber, pageSize)
            .ToArrayAsync();
    }

    public async Task<Article?> GetByIdAsync(Guid id, string requesterUserId)
    {
        return await context.Articles
            .Include(a => a.Author)
                .ThenInclude(a => a.ApplicationUser)
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null && (a.Author.ApplicationUserId == requesterUserId || a.IsPublished));
    }

    public async Task UpdateArticleAsync(Article article, string userToUpdate)
    {
        article.PrepareUpdate(userToUpdate);
        context.Articles.Update(article);
        await context.SaveChangesAsync();
    }
}
