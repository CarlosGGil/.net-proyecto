using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class OrdenItemConfiguration : IEntityTypeConfiguration<OrdenItem>
{
    public void Configure(EntityTypeBuilder<OrdenItem> builder)
    {
        builder.ToTable("orden_items");
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.EntradaTipo).IsRequired().HasMaxLength(100);
        builder.Property(oi => oi.Cantidad).IsRequired();
        builder.Property(oi => oi.PrecioUnitario).HasPrecision(18, 2).IsRequired();
        builder.Property(oi => oi.Subtotal).HasPrecision(18, 2).IsRequired();

        builder.HasOne(oi => oi.Evento)
               .WithMany()
               .HasForeignKey(oi => oi.EventoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(oi => oi.EventoId);
    }
}
