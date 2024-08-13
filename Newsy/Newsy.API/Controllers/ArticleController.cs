using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsy.API.DTOs.Requests;
using Newsy.API.DTOs.Responses;
using Newsy.API.Extensions;
using Newsy.Core.Contracts.Services;
using Newsy.Core.Models;

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
        var getArticlesResult = await articleService.GetArticlesAsync(gridParams.PageNumber, gridParams.PageSize, gridParams.Filters);
        return getArticlesResult.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleAsync(Guid id)
    {
        var getArticleResult = await articleService.GetArticleByIdAsync(id);
        return getArticleResult.ToActionResult(article => mapper.Map<ArticleViewModel>(article));
    }

    [Authorize(Roles = "Author")]
    [HttpPost]
    public async Task<IActionResult> CreateArticleAsync([FromBody] UpsertArticleDto createArticleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createArticleResult = await articleService.CreateArticleAsync(mapper.Map<UpsertArticleServiceModel>(createArticleDto));
        return createArticleResult.ToActionResult();
    }

    [Authorize(Roles = "Author")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticleAsync(Guid id, [FromBody] UpsertArticleDto updateArticleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updateArticleResult = await articleService.UpdateArticleAsync(id, mapper.Map<UpsertArticleServiceModel>(updateArticleDto));
        return updateArticleResult.ToActionResult();
    }

    [Authorize(Roles = "Author")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticleAsync(Guid id)
    {
        var deleteArticleResult = await articleService.DeleteArticleAsync(id);
        return deleteArticleResult.ToActionResult();
    }
}