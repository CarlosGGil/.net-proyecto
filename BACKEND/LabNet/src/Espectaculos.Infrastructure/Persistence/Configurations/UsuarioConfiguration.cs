using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(e => e.id);
        builder.Property(e => e.documento);
        builder.Property(e => e.nombre);
        builder.Property(e => e.apellido);
        builder.Property(e => e.email);
        builder.Property(e => e.estado);
        builder.Property(e => e.credencial);
    }
    
}