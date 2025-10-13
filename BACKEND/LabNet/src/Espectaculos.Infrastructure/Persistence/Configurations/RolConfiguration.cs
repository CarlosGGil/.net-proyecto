using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.RolId);

            builder.Property(r => r.Tipo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Prioridad)
                .IsRequired();

            builder.Property(r => r.FechaAsignado)
                .IsRequired();

            // Relación M:N con Usuario (a través de UsuarioRol)
            builder.HasMany(r => r.UsuarioRoles)
                .WithOne(ur => ur.Rol)
                .HasForeignKey(ur => ur.RolId);
        }
    }
}