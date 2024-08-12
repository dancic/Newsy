using Newsy.Abstractions.Models;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Models;

namespace Newsy.Core.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository authorRepository;
    private readonly IAuthService authService;

    public AuthorService(IAuthorRepository authorRepository,
        IAuthService authService)
    {
        this.authorRepository = authorRepository;
        this.authService = authService;
    }

    public async Task<Author?> GetAuthorAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        return await authorRepository.GetByIdAsync(id, currentUserId);
    }

    public async Task<BasicAuthorModel[]> GetAuthorsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters)
    {
        return await authorRepository.GetAuthorsGridDataAsync(pageNumber, pageSize, filters);
    }

    public async Task<bool> UpdateAuthorAsync(Guid id, UpsertAuthorServiceModel upsertAuthorServiceModel)
    {
        var currentUserId = authService.GetCurrentUserId();
        var authorToUpdate = await authorRepository.GetByIdAsync(id, currentUserId);

        if (authorToUpdate == null || authorToUpdate.ApplicationUser.Id != currentUserId)
        {
            return false;
        }

        authorToUpdate.ApplicationUser.FullName = upsertAuthorServiceModel.Name;
        authorToUpdate.Bio = upsertAuthorServiceModel.Bio;
        authorToUpdate.PrepareUpdate(authorToUpdate.ApplicationUser.FullName);
        await authorRepository.UpdateUserAsync(authorToUpdate);

        return true;
    }
}
