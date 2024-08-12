using Microsoft.EntityFrameworkCore;
using Newsy.Abstractions.Models;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Extensions;
using Newsy.Persistence.Helpers;
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

    public async Task<BasicArticleModel[]> GetArticleGridDataAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, string requesterUserId)
    {
        return await context.Articles
            .Include(a => a.Author)
                .ThenInclude(a => a.ApplicationUser)
            .Where(a => a.DeletedAt == null && (a.Author.ApplicationUserId == requesterUserId || a.IsPublished))
            .Select(a => new BasicArticleModel()
            {
                Id = a.Id,
                Title = a.Title,
                Author = new BasicAuthorModel()
                {
                    Id = a.Author.Id,
                    Name = a.Author.ApplicationUser.FullName
                }
            })
            .Where(FilterExpressionBuilder.GetFiltersExpression<BasicArticleModel>(filters))
            .Paginate(pageNumber, pageSize)
            .OrderBy(a => a.Title)
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
