using Newsy.Abstractions.Models;

namespace Newsy.API.DTOs.Responses;

public class ArticleViewModel : BasicArticleModel
{
    public string Content { get; set; }
}
