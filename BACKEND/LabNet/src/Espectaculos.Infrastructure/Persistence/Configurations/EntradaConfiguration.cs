using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class EntradaConfiguration : IEntityTypeConfiguration<Entrada>
{
    public void Configure(EntityTypeBuilder<Entrada> builder)
    {
        builder.ToTable("entrada");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Tipo).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Precio).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.StockTotal).IsRequired();
        builder.Property(e => e.StockDisponible).IsRequired();
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.Property(e => e.UpdatedAtUtc).IsRequired();

        builder.HasIndex(e => new { e.EventoId, e.Tipo }).IsUnique();
    }
}
