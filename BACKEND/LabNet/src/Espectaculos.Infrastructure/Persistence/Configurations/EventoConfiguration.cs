using Espectaculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Espectaculos.Infrastructure.Persistence.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("eventos");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.Lugar).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Publicado).HasDefaultValue(false);
        builder.Property(e => e.Fecha).IsRequired();
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.Property(e => e.UpdatedAtUtc).IsRequired();

        builder.HasMany(e => e.Entradas)
               .WithOne(x => x.Evento!)
               .HasForeignKey(x => x.EventoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Fecha);
        builder.HasIndex(e => e.Titulo);
        builder.HasIndex(e => e.Lugar);
        builder.HasIndex(e => e.Publicado);
    }
}
