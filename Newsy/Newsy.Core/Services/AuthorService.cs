using FluentResults;
using Newsy.Abstractions;
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

    public async Task<Result<Author?>> GetAuthorAsync(Guid id)
    {
        var currentUserId = authService.GetCurrentUserId();
        return Result.Ok(await authorRepository.GetByIdAsync(id, currentUserId));
    }

    public async Task<Result<BasicAuthorModel[]>> GetAuthorsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters)
    {
        return Result.Ok(await authorRepository.GetAuthorsGridDataAsync(pageNumber, pageSize, filters));
    }

    public async Task<Result> UpdateAuthorAsync(Guid id, UpsertAuthorServiceModel upsertAuthorServiceModel)
    {
        var currentUserId = authService.GetCurrentUserId();
        var authorToUpdate = await authorRepository.GetByIdAsync(id, currentUserId);

        if (authorToUpdate == null)
        {
            return Result.Fail(new Error("There is no author with given id.").WithMetadata(Constants.StatusCodeMetadataName, 404));
        }

        if (authorToUpdate.ApplicationUserId != currentUserId)
        {
            return Result.Fail(new Error("User can update only his profile.").WithMetadata(Constants.StatusCodeMetadataName, 403));
        }

        authorToUpdate.ApplicationUser.FullName = upsertAuthorServiceModel.Name;
        authorToUpdate.Bio = upsertAuthorServiceModel.Bio;
        authorToUpdate.PrepareUpdate(authorToUpdate.ApplicationUser.FullName);
        await authorRepository.UpdateUserAsync(authorToUpdate);

        return Result.Ok();
    }
}
