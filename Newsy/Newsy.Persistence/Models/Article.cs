namespace Newsy.Persistence.Models;

public class Article : DbObject<Guid>
{
    public string Title { get; set; }

    public string Content { get; set; }

    public Guid AuthorId { get; set; }

    public bool IsPublished { get; set; }

    public DateTimeOffset? LastPublishedDateTime { get; set; }

    public Author Author { get; set; }
}
