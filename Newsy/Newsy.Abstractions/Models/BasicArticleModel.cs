namespace Newsy.Abstractions.Models;

public class BasicArticleModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public BasicAuthorModel Author { get; set; }
}
