using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Configurations;

public abstract class DbObjectConfiguration<TEntity, Tid> : IEntityTypeConfiguration<TEntity>
    where TEntity : DbObject<Tid>
    where Tid : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(KeyExpression());

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTimeOffset.UtcNow);

        builder.Property(e => e.CreatedBy)
            .HasColumnType("character varying");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTimeOffset.UtcNow);

        builder.Property(e => e.UpdatedBy)
            .HasColumnType("character varying");
    }

    protected virtual Expression<Func<TEntity, object?>> KeyExpression()
        => e => e.Id;
}
