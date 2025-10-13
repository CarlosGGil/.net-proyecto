using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class BeneficioConfiguration : IEntityTypeConfiguration<Beneficio>
{
    public void Configure(EntityTypeBuilder<Beneficio> builder)
    {
        builder.ToTable("beneficio");
        builder.ToTable("espacio");
        builder.HasKey(e => e.BeneficioId);
        builder.Property(e => e.Tipo).HasConversion<string>().IsRequired();
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Descripcion).IsRequired();
        builder.Property(e => e.VigenciaInicio).IsRequired();
        builder.Property(e => e.VigenciaFin).IsRequired();
        builder.Property(e => e.CupoTotal).IsRequired();
        builder.Property(e => e.CupoPorUsuario).IsRequired();
        builder.Property(e => e.RequiereBiometria).IsRequired();
        builder.Property(e => e.CriterioElegibilidad).IsRequired();
        
        builder.HasMany(e => e.Espacios)
            .WithOne(r => r.Beneficio)
            .HasForeignKey(r => r.EspacioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Usuarios)
            .WithOne(b => b.Beneficio)
            .HasForeignKey(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}