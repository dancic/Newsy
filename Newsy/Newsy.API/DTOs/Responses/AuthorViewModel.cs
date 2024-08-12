using Newsy.Abstractions.Models;

namespace Newsy.API.DTOs.Responses;

public class AuthorViewModel : BasicAuthorModel
{
    public BasicArticleModel[] Articles { get; set; }
    public string Bio { get; set; }
}
