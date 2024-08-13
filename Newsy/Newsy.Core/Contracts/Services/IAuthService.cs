using FluentResults;
using Newsy.Core.Models;

namespace Newsy.Core.Contracts.Services;
public interface IAuthService
{
    Task<Result> RegisterNewUser(RegisterServiceModel registerServiceModel);
    Task<Result<string>> ValidateCredsAndGetTokenAsync(string username, string password);
    string GetCurrentUserId();
}
