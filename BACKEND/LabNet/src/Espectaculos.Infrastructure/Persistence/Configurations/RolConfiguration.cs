using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Espectaculos.Domain.Entities;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.RolId);

        builder.Property(r => r.Tipo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Prioridad)
            .IsRequired();

        builder.Property(r => r.FechaAsignado)
            .IsRequired();
        builder.HasMany(r => r.UsuarioRoles)
            .WithOne(ur => ur.Rol)
            .HasForeignKey(ur => ur.RolId);

        builder.HasIndex(r => r.Tipo);
    }
}