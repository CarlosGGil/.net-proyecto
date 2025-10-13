using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class EventoAccesoConfiguration : IEntityTypeConfiguration<EventoAcceso>
{
    public void Configure(EntityTypeBuilder<EventoAcceso> builder)
    {
        builder.ToTable("evento_acceso");
        builder.HasKey(e => e.EventoId);
        builder.Property(e => e.MomentoDeAcceso).IsRequired();
        builder.Property(e => e.CredencialId).IsRequired();
        builder.Property(e => e.EspacioId).IsRequired();
        builder.Property(e => e.Resultado).HasConversion<string>().IsRequired();
        builder.Property(e => e.Motivo).HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.Modo).HasConversion<string>().IsRequired();
        builder.Property(e => e.Firma).HasMaxLength(1024).IsRequired(false);
        
        builder.HasOne(e => e.Espacio)
               .WithMany(s => s.EventoAccesos)
               .HasForeignKey(e => e.EspacioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Credencial)
               .WithMany(c => c.EventosAcceso)
               .HasForeignKey(e => e.CredencialId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
