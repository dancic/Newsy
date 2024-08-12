using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy.API.DTOs.Requests;
using Newsy.API.DTOs.Responses;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;
using Newsy.Persistence.Models;

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
        var authors = await authorService.GetAuthorsAsync(gridParams.PageNumber, gridParams.PageSize);
        return Ok(mapper.Map<Author[], BasicAuthorViewModel[]>(authors));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorAsync(Guid id)
    {
        var author = await authorService.GetAuthorAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<AuthorViewModel>(author));
    }

    [Authorize(Roles = "Author")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthorAsync(Guid id, [FromBody] UpsertAuthorDto updateAuthorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await authorService.UpdateAuthorAsync(id, mapper.Map<UpsertAuthorServiceModel>(updateAuthorDto));
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}