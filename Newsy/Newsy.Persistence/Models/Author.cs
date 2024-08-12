namespace Newsy.Persistence.Models;

public class Author : DbObject<Guid>
{
    public string Bio { get; set; }

    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; }

    public ICollection<Article> Articles { get; set; }
}
