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
public class ArticleController : ControllerBase
{
    private readonly IArticleService articleService;
    private readonly IMapper mapper;

    public ArticleController(IArticleService articleService,
        IMapper mapper)
    {
        this.articleService = articleService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticlesAsync([FromQuery] GridDto gridParams)
    {
        var articles = await articleService.GetArticlesAsync(gridParams.PageNumber, gridParams.PageSize, gridParams.Filters);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleAsync(Guid id)
    {
        var article = await articleService.GetArticleByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<ArticleViewModel>(article));
    }

    [Authorize(Roles = "Author")]
    [HttpPost]
    public async Task<IActionResult> CreateArticleAsync([FromBody] UpsertArticleDto createArticleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var articleId = await articleService.CreateArticleAsync(mapper.Map<UpsertArticleServiceModel>(createArticleDto));
        return Ok(new { id = articleId });
    }

    [Authorize(Roles = "Author")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticleAsync(Guid id, [FromBody] UpsertArticleDto updateArticleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await articleService.UpdateArticleAsync(id, mapper.Map<UpsertArticleServiceModel>(updateArticleDto));
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Author")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticleAsync(Guid id)
    {
        var result = await articleService.DeleteArticleAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}