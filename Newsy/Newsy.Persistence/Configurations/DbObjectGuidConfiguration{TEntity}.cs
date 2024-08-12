using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newsy.Persistence.Models;

namespace Newsy.Persistence.Configurations;
public abstract class DbObjectGuidConfiguration<TEntity> : DbObjectConfiguration<TEntity, Guid>
    where TEntity : DbObject<Guid>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();
    }
}
