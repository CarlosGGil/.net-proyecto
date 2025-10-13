using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class EspacioConfiguration : IEntityTypeConfiguration<Espacio>
{
    public void Configure(EntityTypeBuilder<Espacio> builder)
    {
        builder.ToTable("espacio");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Activo).IsRequired();
        builder.Property(e => e.Tipo).IsRequired();
        builder.Property(e => e.Modo).IsRequired();
        
        builder.HasMany(e => e.Reglas)
            .WithOne(r => r.Espacio)
            .HasForeignKey(r => r.EspacioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Beneficios)
            .WithOne(b => b.Espacio)
            .HasForeignKey(b => b.EspacioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}