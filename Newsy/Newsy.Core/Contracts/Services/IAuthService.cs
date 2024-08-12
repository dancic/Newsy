using Newsy.Core.Models;

namespace Newsy.Core.Contracts.Services;
public interface IAuthService
{
    Task<bool> RegisterNewUser(RegisterServiceModel registerServiceModel);
    Task<string?> ValidateCredsAndGetTokenAsync(string username, string password);
    string GetCurrentUserId();
}
