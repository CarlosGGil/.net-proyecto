using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class BeneficioUsuarioConfiguration: IEntityTypeConfiguration<BeneficioUsuario>
{
    public void Configure(EntityTypeBuilder<BeneficioUsuario> builder)
    {
        builder.ToTable("beneficio_usuario");
        // PK compuesta (Beneficio + UsuarioId)
        builder.HasKey(x => new { x.BeneficioId, x.UsuarioId});

        builder.HasOne(x => x.Usuario)
            .WithMany(e => e.Beneficios)     // la colección en Usuario
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Beneficio)
            .WithMany(r => r.Usuarios)   // colección en Beneficio
            .HasForeignKey(x => x.BeneficioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}