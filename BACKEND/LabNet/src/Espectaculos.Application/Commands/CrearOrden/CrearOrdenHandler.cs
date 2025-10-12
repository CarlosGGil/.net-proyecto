using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Common.Exceptions;
using Espectaculos.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Espectaculos.Application.Commands.CrearOrden;

public class CrearOrdenHandler(
    IUnitOfWork uow,
    IValidator<CrearOrdenCommand> validator,
    ILogger<CrearOrdenHandler>? logger)
{
    private readonly ILogger<CrearOrdenHandler> _logger = logger ?? NullLogger<CrearOrdenHandler>.Instance;

    public async Task<CrearOrdenResult> HandleAsync(CrearOrdenCommand command, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(command, ct);
        var total = 0m;
        var nowUtc = DateTime.UtcNow;
        var orden = new Orden
        {
            Id = Guid.NewGuid(),
            EmailComprador = command.EmailComprador.Trim(),
            Fecha = nowUtc,
            Total = 0m,
            Items = new List<OrdenItem>()
        };

        foreach (var item in command.Items)
        {
            var evento = await uow.Eventos.GetByIdAsync(item.EventoId, ct);
            if (evento is null)
                throw new NotFoundException("El evento no existe.");

            // Validar disponibilidad (regla unificada): fecha futura Y stock total > 0
            var fechaUtc = evento.Fecha.Kind == DateTimeKind.Utc
                ? evento.Fecha
                : DateTime.SpecifyKind(evento.Fecha, DateTimeKind.Utc);

            var stockTotal = (evento.Entradas?.Sum(x => (int?)x.StockDisponible) ?? 0);

            if (fechaUtc <= nowUtc || stockTotal <= 0)
            {
                _logger.LogWarning("CHECKOUT_GUARD: stockTotal={StockTotal}, fecha={Fecha:o} (Kind={Kind}), nowUtc={Now:o}, eventoId={EventoId}",
                    stockTotal, evento.Fecha, evento.Fecha.Kind, nowUtc, evento.Id);
                throw new ConflictException("El evento no está disponible o la fecha ya pasó.");
            }

            var entrada = await uow.Entradas.GetByEventoAndTipoAsync(item.EventoId, item.EntradaTipo.Trim(), ct);
            if (entrada is null)
                throw new NotFoundException("El tipo de entrada no existe para el evento solicitado.");
            // if (item.Cantidad <= 0)
            //     throw new ConflictException("La cantidad debe ser mayor a 0.");

            if (item.Cantidad > entrada.StockDisponible)
                throw new ConflictException("Stock insuficiente para la entrada seleccionada.");

            if (entrada.StockDisponible - item.Cantidad < 0)
                throw new ConflictException("La operación dejaría stock negativo.");

            // Descontar stock
            entrada.StockDisponible -= item.Cantidad;
            uow.Entradas.Update(entrada);

            var subtotal = entrada.Precio * item.Cantidad;
            total += subtotal;

            orden.Items.Add(new OrdenItem
            {
                Id = Guid.NewGuid(),
                OrdenId = orden.Id,
                EventoId = item.EventoId,
                EntradaTipo = item.EntradaTipo,
                Cantidad = item.Cantidad,
                PrecioUnitario = entrada.Precio,
                Subtotal = subtotal
            });
        }

        orden.Total = total;

        // Nota: Para manejar concurrencia/stock negativo en alta carga, considerar transacciones y/o tokens de concurrencia (rowversion).
        await uow.Ordenes.AddAsync(orden, ct);
        await uow.SaveChangesAsync(ct);

        return new CrearOrdenResult
        {
            Id = orden.Id,
            EmailComprador = orden.EmailComprador,
            Fecha = orden.Fecha,
            Total = orden.Total,
            Items = orden.Items.Select(i => new CrearOrdenResult.Item
            {
                EventoId = i.EventoId,
                EntradaTipo = i.EntradaTipo,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
