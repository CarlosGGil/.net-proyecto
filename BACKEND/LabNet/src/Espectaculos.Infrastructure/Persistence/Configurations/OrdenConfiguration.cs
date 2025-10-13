using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class OrdenConfiguration : IEntityTypeConfiguration<Orden>
{
    public void Configure(EntityTypeBuilder<Orden> builder)
    {
        builder.ToTable("orden");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.EmailComprador).IsRequired().HasMaxLength(256);
        builder.Property(o => o.Total).HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.Fecha).IsRequired();
        builder.Property(o => o.CreatedAtUtc).IsRequired();
        builder.Property(o => o.UpdatedAtUtc).IsRequired();
        builder.Property(o => o.RedeemedAtUtc);
        builder.Property(o => o.RedeemedBy).HasMaxLength(128);

        builder.HasMany(o => o.Items)
               .WithOne(i => i.Orden!)
               .HasForeignKey(i => i.OrdenId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.EmailComprador);
        builder.HasIndex(o => o.Fecha);
    }
}
