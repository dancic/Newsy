using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Newsy.Abstractions;
using Newsy.Core.Models;
using Newsy.Core.Services;
using Newsy.Persistence.Contracts.Services;
using Newsy.Persistence.Models;
using Newsy.UnitTests.Helpers;
using System.Security.Claims;

namespace Newsy.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IConfiguration> configurationMock;
    private readonly Mock<FakeUserManager> userManagerMock;
    private readonly Mock<FakeSignInManager> signInManagerMock;
    private readonly Mock<IAuthorRepository> authorRepositoryMock;
    private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
    private readonly AuthService authService;

    public AuthServiceTests()
    {
        configurationMock = new Mock<IConfiguration>();
        authorRepositoryMock = new Mock<IAuthorRepository>();
        httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        userManagerMock = new Mock<FakeUserManager>();
        signInManagerMock = new Mock<FakeSignInManager>();

        authService = new AuthService(
            configurationMock.Object,
            userManagerMock.Object,
            signInManagerMock.Object,
            authorRepositoryMock.Object,
            httpContextAccessorMock.Object
        );
    }

    [Fact]
    public async Task RegisterNewUser_ShouldReturnOk_WhenUserIsRegistered()
    {
        // Arrange
        var registerModel = new RegisterServiceModel
        {
            FullName = "Test User",
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Test@123",
            IsAuthor = true,
            Bio = "Author bio"
        };

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        authorRepositoryMock.Setup(ar => ar.CreateAuthorAsync(It.IsAny<Author>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await authService.RegisterNewUser(registerModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterNewUser_ShouldReturnFailure_WhenUserCreationFails()
    {
        // Arrange
        var registerModel = new RegisterServiceModel
        {
            FullName = "Test User",
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Test@123"
        };

        var identityError = new IdentityError { Code = "UserCreationFailed" };
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await authService.RegisterNewUser(registerModel);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterNewUser_ShouldAddToAuthorRole_WhenUserIsAuthor()
    {
        // Arrange
        var registerModel = new RegisterServiceModel
        {
            FullName = "Test Author",
            Username = "testauthor",
            Email = "testauthor@example.com",
            Password = "Test@123",
            IsAuthor = true,
            Bio = "Author bio"
        };

        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Constants.AuthorRoleName))
            .ReturnsAsync(IdentityResult.Success);
        authorRepositoryMock.Setup(ar => ar.CreateAuthorAsync(It.IsAny<Author>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await authService.RegisterNewUser(registerModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Constants.AuthorRoleName), Times.Once);
        authorRepositoryMock.Verify(ar => ar.CreateAuthorAsync(It.IsAny<Author>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCredsAndGetTokenAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var username = "testuser";
        var password = "Test@123";
        var user = new ApplicationUser { UserName = username };

        userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, password, false, false))
            .ReturnsAsync(SignInResult.Success);
        userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey!12312312312312312313123");
        configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
        configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testaudience");

        // Act
        var result = await authService.ValidateCredsAndGetTokenAsync(username, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateCredsAndGetTokenAsync_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "Test@123";

        userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await authService.ValidateCredsAndGetTokenAsync(username, password);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateCredsAndGetTokenAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var username = "testuser";
        var password = "WrongPassword";
        var user = new ApplicationUser { UserName = username };

        userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, password, false, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await authService.ValidateCredsAndGetTokenAsync(username, password);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void GetCurrentUserId_ShouldReturnUserId_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        httpContextAccessorMock.Setup(h => h.HttpContext.User)
            .Returns(claimsPrincipal);
        userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(userId);

        // Act
        var result = authService.GetCurrentUserId();

        // Assert
        result.Should().Be(userId);
    }
}
