using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class SincronizacionConfiguration: IEntityTypeConfiguration<Sincronizacion>
{
    public void Configure(EntityTypeBuilder<Sincronizacion> builder)
    {
        builder.ToTable("sincronizacion");
        builder.HasKey(e => e.SincronizacionId);
        builder.Property(e => e.DispositivoId).IsRequired();
        builder.Property(e => e.CreadoEn).IsRequired();
        builder.Property(e => e.CantidadItems).IsRequired();
        builder.Property(e => e.Tipo).HasConversion<string>().IsRequired();
        builder.Property(e => e.Estado).IsRequired(false);
        builder.Property(e => e.DetalleError).IsRequired(false);
        builder.Property(e => e.Checksum).IsRequired(false);
        
        builder.HasOne(e => e.Dispositivo)
            .WithMany(x  => x.Sincronizaciones)
            .HasForeignKey(e => e.DispositivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}