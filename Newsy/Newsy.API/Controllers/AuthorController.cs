using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy.Abstractions;
using Newsy.API.DTOs.Requests;
using Newsy.API.DTOs.Responses;
using Newsy.API.Extensions;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;

namespace Newsy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService authorService;
    private readonly IMapper mapper;

    public AuthorController(IAuthorService authorService,
        IMapper mapper)
    {
        this.authorService = authorService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthorsAsync([FromQuery] GridDto gridParams)
    {
        var authorsResult = await authorService.GetAuthorsAsync(gridParams.PageNumber, gridParams.PageSize, gridParams.Filters);
        return authorsResult.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorAsync(Guid id)
    {
        var getAuthorResult = await authorService.GetAuthorAsync(id);
        return getAuthorResult.ToActionResult(author => mapper.Map<AuthorViewModel>(author));
    }

    [Authorize(Roles = Constants.AuthorRoleName)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthorAsync(Guid id, [FromBody] UpsertAuthorDto updateAuthorDto)
    {
        var updateAuthorResult = await authorService.UpdateAuthorAsync(id, mapper.Map<UpsertAuthorServiceModel>(updateAuthorDto));
        return updateAuthorResult.ToActionResult();
    }
}