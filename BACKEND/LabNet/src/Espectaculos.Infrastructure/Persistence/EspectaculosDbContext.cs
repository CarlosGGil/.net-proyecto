using Espectaculos.Domain.Entities;
using Espectaculos.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic; 
namespace Espectaculos.Infrastructure.Persistence;

public sealed class EspectaculosDbContext : DbContext
{
    private readonly AuditableEntitySaveChangesInterceptor? _auditable;
    private readonly AvailabilityRecomputeInterceptor? _availability;
    private readonly IEnumerable<IInterceptor>? _extras;

    public EspectaculosDbContext(
        DbContextOptions<EspectaculosDbContext> options,
        AuditableEntitySaveChangesInterceptor? auditableInterceptor = null,
        AvailabilityRecomputeInterceptor? availabilityRecomputeInterceptor = null,
        IEnumerable<IInterceptor>? extraInterceptors = null)
        : base(options)
    {
        _auditable = auditableInterceptor;
        _availability = availabilityRecomputeInterceptor;
        _extras = extraInterceptors;
    }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Entrada> Entradas => Set<Entrada>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<OrdenItem> OrdenItems => Set<OrdenItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EspectaculosDbContext).Assembly);

        // ⬇️⬇️⬇️ CONFIGURACIÓN PARA Dictionary<string,string> ⬇️⬇️⬇️
        modelBuilder.SharedTypeEntity<Dictionary<string, string>>(
            name: "NotificacionMetadato",
            buildAction: b =>
            {
                // columnas para el diccionario
                b.IndexerProperty<string>("Key").HasColumnName("Clave");
                b.IndexerProperty<string>("Value").HasColumnName("Valor");

                // PK compuesta: FK al owner + clave del diccionario
                b.HasKey("NotificacionId", "Clave");
                b.ToTable("NotificacionMetadatos");
            });

        modelBuilder.Entity<Notificacion>()
            .OwnsMany<Dictionary<string, string>>(
                navigationName: "Metadatos",           // propiedad en tu clase Notificacion
                ownedTypeName: "NotificacionMetadato", // mismo nombre que arriba
                buildAction: b =>
                {
                    b.WithOwner().HasForeignKey("NotificacionId");
                    // la PK ya está arriba, pero si querés repetirla acá no rompe
                    // b.HasKey("NotificacionId", "Clave");
                });
        // ⬆️⬆️⬆️ FIN CONFIGURACIÓN ⬆️⬆️⬆️

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_auditable   is not null) optionsBuilder.AddInterceptors(_auditable);
        if (_availability is not null) optionsBuilder.AddInterceptors(_availability);
        if (_extras       is not null) optionsBuilder.AddInterceptors(_extras);

        base.OnConfiguring(optionsBuilder);
    }
}

/* ─────────────────────────────────────────────────────────────────────────────
   LEGACY VERSION (kept here for reference ONLY; commented so it does not compile)
   ─────────────────────────────────────────────────────────────────────────────
using Espectaculos.Domain.Common;
using Espectaculos.Domain.Entities;
using Espectaculos.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Espectaculos.Infrastructure.Persistence;

public class EspectaculosDbContext : DbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableInterceptor;

    public EspectaculosDbContext(DbContextOptions<EspectaculosDbContext> options,
                                 AuditableEntitySaveChangesInterceptor auditableInterceptor)
        : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Entrada> Entradas => Set<Entrada>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<OrdenItem> OrdenItems => Set<OrdenItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EspectaculosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
        optionsBuilder.AddInterceptors(new AvailabilityRecomputeInterceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
*/
