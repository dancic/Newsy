namespace Newsy.Core.Models;

public class UpsertArticleServiceModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublished { get; set; }
}
