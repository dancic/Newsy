using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasMany(a => a.Articles)
            .WithOne(a => a.Author)
            .HasForeignKey(a => a.AuthorId);

        builder.HasOne(a => a.ApplicationUser)
            .WithOne(u => u.Author)
            .HasForeignKey<Author>(a => a.ApplicationUserId)
            .IsRequired();
    }
}
