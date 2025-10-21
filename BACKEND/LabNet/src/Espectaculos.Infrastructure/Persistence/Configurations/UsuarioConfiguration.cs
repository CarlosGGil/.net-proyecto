using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario");
            builder.HasKey(u => u.UsuarioId);
            builder.Property(u => u.Documento).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(150);
            
            builder.HasMany(u => u.Canjes)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(u => u.Dispositivos)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.Beneficios)
                .WithOne(bu => bu.Usuario)
                .HasForeignKey(bu => bu.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(u => u.UsuarioRoles)
                .WithOne(ur => ur.Usuario)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}