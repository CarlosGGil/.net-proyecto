using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class BeneficioConfiguration : IEntityTypeConfiguration<Beneficio>
{
    public void Configure(EntityTypeBuilder<Beneficio> builder)
    {
        builder.ToTable("beneficio");
        builder.HasKey(e => e.BeneficioId);
        builder.Property(e => e.Tipo).HasConversion<string>().IsRequired();
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
    // Estas propiedades son opcionales en el modelo de dominio — mapear como nullable en DB
    builder.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired(false);
    builder.Property(e => e.VigenciaInicio).IsRequired(false);
    builder.Property(e => e.VigenciaFin).IsRequired(false);
    builder.Property(e => e.CupoTotal).IsRequired(false);
    builder.Property(e => e.CupoPorUsuario).IsRequired(false);
    builder.Property(e => e.RequiereBiometria).IsRequired(); // requerido (bool no-nullable)
    builder.Property(e => e.CriterioElegibilidad).HasMaxLength(1000).IsRequired(false);
        
        builder.HasMany(e => e.Espacios)
            .WithOne(r => r.Beneficio)
            .HasForeignKey(r => r.BeneficioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Usuarios)
            .WithOne(b => b.Beneficio)
            .HasForeignKey(b => b.BeneficioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}