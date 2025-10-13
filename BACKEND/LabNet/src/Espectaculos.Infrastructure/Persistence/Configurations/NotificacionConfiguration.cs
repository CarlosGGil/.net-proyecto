using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class NotificacionConfiguration: IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("notificacion");
        builder.HasKey(e => e.NotificacionId);
        builder.Property(e => e.DispositivoId).IsRequired();
        builder.Property(e => e.Tipo).IsRequired();
        builder.Property(e => e.Titulo).IsRequired();
        builder.Property(e => e.Cuerpo).HasConversion<string>().IsRequired();
        builder.Property(e => e.ProgramadaPara).IsRequired(false);
        builder.Property(e => e.Estado).IsRequired(false);
        builder.Property(e => e.Canales).IsRequired(false);
        builder.Property(e => e.Metadatos).IsRequired(false);
        
        builder.HasOne(e => e.Dispositivo)
            .WithMany(x  => x.Notificaciones)
            .HasForeignKey(e => e.DispositivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}