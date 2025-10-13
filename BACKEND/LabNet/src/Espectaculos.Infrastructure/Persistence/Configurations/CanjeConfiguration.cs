using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class CanjeConfiguration: IEntityTypeConfiguration<Canje>
{
    public void Configure(EntityTypeBuilder<Canje> builder)
    {
        builder.ToTable("canje");
        builder.HasKey(e => e.CanjeId);
        builder.Property(e => e.BeneficioId).IsRequired();
        builder.Property(e => e.UsuarioId).IsRequired();
        builder.Property(e => e.Fecha).IsRequired();
        builder.Property(e => e.Estado).HasConversion<string>().IsRequired();
        builder.Property(e => e.VerificacionBiometrica).IsRequired(false);
        builder.Property(e => e.Firma).HasMaxLength(1024).IsRequired(false);
        
        builder.HasOne(e => e.Beneficio)
            .WithMany()
            .HasForeignKey(e => e.BeneficioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Usuario)
            .WithMany(c => c.Canjes)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}