using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newsy.API.DTOs.Requests;
using Newsy.API.Extensions;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;

namespace Newsy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IMapper mapper;

    public UserController(IAuthService authService,
        IMapper mapper)
    {
        this.authService = authService;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var registrationServiceModel = mapper.Map<RegisterServiceModel>(registerDto);
        var registrationResult = await authService.RegisterNewUser(registrationServiceModel);

        return registrationResult.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var tokenResult = await authService.ValidateCredsAndGetTokenAsync(loginDto.Username, loginDto.Password);
        return tokenResult.ToActionResult();
    }
}
