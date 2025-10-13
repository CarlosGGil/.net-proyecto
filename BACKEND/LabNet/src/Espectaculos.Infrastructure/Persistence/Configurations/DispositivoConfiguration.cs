using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations
{
    public class DispositivoConfiguration : IEntityTypeConfiguration<Dispositivo>
    {
        public void Configure(EntityTypeBuilder<Dispositivo> builder)
        {
            builder.ToTable("dispositivo");

            builder.HasKey(d => d.DispositivoId);

            builder.Property(d => d.NumeroTelefono)
                .HasMaxLength(20);

            builder.Property(d => d.Plataforma)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.HuellaDispositivo)
                .IsRequired();

            builder.Property(d => d.BiometriaHabilitada)
                .IsRequired();

            builder.Property(d => d.Estado)
                .IsRequired();

            // Relación con Usuario
            builder.HasOne(d => d.Usuario)
                .WithMany(u => u.Dispositivos)
                .HasForeignKey(d => d.UsuarioId);
            
            builder.HasMany(e => e.Sincronizaciones)
                .WithOne(c => c.Dispositivo)
                .HasForeignKey(e => e.SincronizacionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(e => e.Notificaciones)
                .WithOne(c => c.Dispositivo)
                .HasForeignKey(e => e.NotificacionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}