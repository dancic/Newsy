using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newsy.Abstractions;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Newsy.Core.Services;
public class AuthService : IAuthService
{
    private readonly IConfiguration configuration;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IAuthorRepository authorRepository;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthService(IConfiguration configuration,
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        IAuthorRepository authorRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        this.configuration = configuration;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.authorRepository = authorRepository;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> RegisterNewUser(RegisterServiceModel registerServiceModel)
    {
        var user = new ApplicationUser
        {
            FullName = registerServiceModel.FullName,
            UserName = registerServiceModel.Username,
            Email = registerServiceModel.Email
        };

        var result = await userManager.CreateAsync(user, registerServiceModel.Password);
        if (!result.Succeeded)
        {
            return Result.Fail(new Error(string.Join(", ", result.Errors.Select(e => e.Code))).WithMetadata(Constants.StatusCodeMetadataName, 400));
        }

        if (registerServiceModel.IsAuthor)
        {
            await userManager.AddToRoleAsync(user, "Author");
            var author = new Author() { Bio = registerServiceModel.Bio!, ApplicationUserId = user.Id };
            author.PrepareUpdate();
            await authorRepository.CreateAuthorAsync(author);
        }

        return Result.Ok();
    }

    public async Task<Result<string>> ValidateCredsAndGetTokenAsync(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
        {
            return Result.Fail(new Error("User with given username does not exist.").WithMetadata(Constants.StatusCodeMetadataName, 401));
        }

        var result = await signInManager.PasswordSignInAsync(user, password, false, false);
        if (!result.Succeeded)
        {
            return Result.Fail(new Error("Unable to login with provided credentials.").WithMetadata(Constants.StatusCodeMetadataName, 401));
        }

        return Result.Ok(await GenerateJwtToken(user));
    }

    public string GetCurrentUserId() => userManager.GetUserId(httpContextAccessor.HttpContext.User);

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(300),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
