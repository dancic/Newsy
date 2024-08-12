using Microsoft.AspNetCore.Identity;

namespace Newsy.Persistence.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }

    public Author? Author { get; set; }

    public bool IsAuthor => Author != null;
}
