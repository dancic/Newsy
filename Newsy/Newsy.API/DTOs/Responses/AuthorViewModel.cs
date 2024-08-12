namespace Newsy.API.DTOs.Responses;

public class AuthorViewModel : BasicAuthorViewModel
{
    public BasicArticleViewModel[] Articles { get; set; }
}
