using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newsy.Persistence.Configurations;
using Newsy.Persistence.Models;

namespace Newsy.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ArticleConfiguration());
        builder.ApplyConfiguration(new AuthorConfiguration());

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole 
            { 
                Id = Guid.NewGuid().ToString(), 
                Name = "Author", 
                NormalizedName = "AUTHOR".ToUpper() 
            }
        );
    }
}
