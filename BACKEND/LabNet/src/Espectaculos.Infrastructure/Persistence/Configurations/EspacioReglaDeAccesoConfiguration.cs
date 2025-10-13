using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class EspacioReglaDeAccesoConfiguration : IEntityTypeConfiguration<EspacioReglaDeAcceso>
{
    public void Configure(EntityTypeBuilder<EspacioReglaDeAcceso> builder)
    {
        builder.ToTable("espacio_regla_de_acceso");

        // PK compuesta (EspacioId + ReglaDeAccesoId)
        builder.HasKey(x => new { x.EspacioId, x.ReglaId });

        builder.HasOne(x => x.Espacio)
            .WithMany(e => e.Reglas)     // la colección en Espacio
            .HasForeignKey(x => x.EspacioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Regla)
            .WithMany(r => r.Espacios)   // colección en ReglaDeAcceso
            .HasForeignKey(x => x.ReglaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
