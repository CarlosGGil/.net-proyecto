using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class BeneficioEspacioConfiguration: IEntityTypeConfiguration<BeneficioEspacio>
{
    public void Configure(EntityTypeBuilder<BeneficioEspacio> builder)
    {
        builder.ToTable("beneficio_espacio");
        // PK compuesta (Beneficio + EspacioId)
        builder.HasKey(x => new { x.BeneficioId, x.EspacioId});

        builder.HasOne(x => x.Espacio)
            .WithMany(e => e.Beneficios)     // la colección en Espacio
            .HasForeignKey(x => x.EspacioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Beneficio)
            .WithMany(r => r.Espacios)   // colección en Beneficio
            .HasForeignKey(x => x.BeneficioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}