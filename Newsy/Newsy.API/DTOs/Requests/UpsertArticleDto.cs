namespace Newsy.API.DTOs.Requests;

public class UpsertArticleDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublished { get; set; }
}
