using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Commands.CrearOrden;
using Espectaculos.Application.Common.Exceptions;
using FluentValidation;
using Espectaculos.WebApi.Security;
using Espectaculos.WebApi.Endpoints.Models;

namespace Espectaculos.WebApi.Endpoints;

public static class OrdenesEndpoints
{
    public static void MapOrdenesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("ordenes");

        group.MapPost("/crear", async (
            CrearOrdenCommand command,
            IUnitOfWork uow,
            IValidator<CrearOrdenCommand> validator,
            ILoggerFactory loggerFactory,
            IValidationTokenService tokenService,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("Ordenes");
            var handler = new CrearOrdenHandler(uow, validator, loggerFactory.CreateLogger<CrearOrdenHandler>());

            try
            {
                var result = await handler.HandleAsync(command, ct);
                var token = tokenService.Generate(result.Id, null);

                return Results.Created($"/api/ordenes/{result.Id}", new
                {
                    id = result.Id,
                    emailComprador = result.EmailComprador,
                    fecha = result.Fecha,
                    total = result.Total,
                    items = result.Items,
                    token,
                    redeemedAtUtc = (DateTime?)null
                });
            }
            catch (ValidationException vex)
            {
                logger.LogWarning(vex, "Validación fallida al crear orden");
                var errors = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.BadRequest(new { errors });
            }
            catch (NotFoundException nf)
            {
                logger.LogInformation(nf, "Recurso no encontrado al crear orden");
                return Results.Problem(
                    statusCode: 404,
                    type: "https://httpstatuses.com/404",
                    title: "No encontrado",
                    detail: nf.Message);
            }
            catch (ConflictException cx)
            {
                logger.LogWarning(cx, "Conflicto de negocio al crear orden");
                return Results.Problem(
                    statusCode: 409,
                    type: "https://httpstatuses.com/409",
                    title: "Conflicto",
                    detail: cx.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al crear orden");
                return Results.Problem(statusCode: 500, title: "Error interno del servidor");
            }
        })
        .WithName("CrearOrden")
        .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IUnitOfWork uow,
            ILoggerFactory loggerFactory,
            IValidationTokenService tokenService,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("Ordenes");
            var orden = await uow.Ordenes.GetByIdAsync(id, ct);
            if (orden is null)
            {
                logger.LogInformation("Orden no encontrada: {Id}", id);
                return Results.Problem(
                    statusCode: 404,
                    type: "https://httpstatuses.com/404",
                    title: "No encontrado",
                    detail: "La orden no existe.");
            }

            var token = tokenService.Generate(orden.Id, null);
            return Results.Ok(new
            {
                id = orden.Id,
                emailComprador = orden.EmailComprador,
                fecha = orden.Fecha,
                total = orden.Total,
                items = orden.Items.Select(i => new {
                    eventoId = i.EventoId, entradaTipo = i.EntradaTipo, cantidad = i.Cantidad,
                    precioUnitario = i.PrecioUnitario, subtotal = i.Subtotal
                }),
                token,
                redeemedAtUtc = orden.RedeemedAtUtc
            });
        })
        .WithName("GetOrden")
        .WithOpenApi();

        group.MapPost("/redeem", async (
            RedeemRequestDto dto,
            IUnitOfWork uow,
            ILoggerFactory loggerFactory,
            IValidationTokenService tokenService,
            CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("Ordenes");
            var check = tokenService.Validate(dto.Token);

            if (check.Status == TokenCheckStatus.Invalid)
            {
                logger.LogWarning("Redeem token inválido: {Detail}", check.Detail);
                return Results.Problem(
                    statusCode: 401,
                    type: "https://httpstatuses.com/401",
                    title: "Token inválido",
                    detail: "El token es malformado o su firma es inválida.");
            }
            if (check.Status == TokenCheckStatus.Expired)
            {
                logger.LogInformation("Redeem token expirado para orden {OrderId}", check.OrderId);
                return Results.Problem(
                    statusCode: 403,
                    type: "https://httpstatuses.com/403",
                    title: "Token expirado",
                    detail: "El token ha expirado.");
            }

            var id = check.OrderId!.Value;
            var orden = await uow.Ordenes.GetByIdAsync(id, ct);
            if (orden is null)
            {
                return Results.Problem(
                    statusCode: 404,
                    type: "https://httpstatuses.com/404",
                    title: "No encontrado",
                    detail: "La orden no existe.");
            }

            if (orden.RedeemedAtUtc is null)
            {
                orden.RedeemedAtUtc = DateTime.UtcNow;
                uow.Ordenes.Update(orden);
                await uow.SaveChangesAsync(ct);
            }

            return Results.Ok(new
            {
                id = orden.Id,
                emailComprador = orden.EmailComprador,
                fecha = orden.Fecha,
                total = orden.Total,
                items = orden.Items.Select(i => new {
                    eventoId = i.EventoId, entradaTipo = i.EntradaTipo, cantidad = i.Cantidad,
                    precioUnitario = i.PrecioUnitario, subtotal = i.Subtotal
                }),
                token = dto.Token,
                redeemedAtUtc = orden.RedeemedAtUtc
            });
        })
        .WithName("RedeemOrden")
        .WithOpenApi();
    }
}
