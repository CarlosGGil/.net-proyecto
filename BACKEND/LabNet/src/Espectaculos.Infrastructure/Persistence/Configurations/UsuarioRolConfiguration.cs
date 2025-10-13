using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("UsuarioRol");

        builder.HasKey(ur => new { ur.UsuarioId, ur.RolId });

        builder.HasOne(ur => ur.Usuario)
            .WithMany(u => u.roles)
            .HasForeignKey(ur => ur.UsuarioId);

        builder.HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(ur => ur.RolId);
    }
}
