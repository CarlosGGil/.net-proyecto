using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Commands.CrearUsuario;
using Espectaculos.Application.Common.Exceptions;
using Espectaculos.Application.Usuarios.Commands.CreateUsuario;
using Espectaculos.Application.Usuarios.Commands.DeleteUsuario;
using Espectaculos.Application.Usuarios.Commands.UpdateUsuario;
using Espectaculos.Application.Usuarios.Queries.ListarUsuarios;
using FluentValidation;
using Espectaculos.WebApi.Security;
using Espectaculos.WebApi.Endpoints.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Espectaculos.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("usuarios");

        group.MapGet("/test", () => { return Results.Ok("test"); }).WithName("Pruebitusuario")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new ListarUsuariosQuery();
            var usuarios = await mediator.Send(query);

            return Results.Ok(usuarios);
        }).WithName("ListarUsuarios").WithOpenApi();
        
        
        group.MapDelete("/", async ([FromBody] DeleteUsuarioCommand command, [FromServices]IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.NoContent();
        });
        
        group.MapPost("/", async ([FromBody] CreateUsuarioCommand command, [FromServices]IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.NoContent();
        });
        
        

        
        
        /**
        group.MapPost("/crear", async (
                [FromBody] CrearUsuarioCommand command,
                CrearUsuarioHandler handler,
                IValidationTokenService tokenService,
                ILoggerFactory loggerFactory,
                CancellationToken ct) =>
            {
                var logger = loggerFactory.CreateLogger("Usuarios");
                try
                {
                    // Handler returns the created UsuarioId (Guid)
                    var usuarioId = await handler.HandleAsync(command, ct);
                    var token = tokenService.Generate(usuarioId, null);
                    return Results.Created($"/api/usuarios/{usuarioId}", new { id = usuarioId, token });
                }
                catch (FluentValidation.ValidationException vex)
                {
                    logger.LogWarning(vex, "Validación fallida al crear usuario");
                    var errors = vex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                    return Results.BadRequest(new { errors });
                }
                catch (NotFoundException nf)
                {
                    logger.LogInformation(nf, "Recurso no encontrado al crear Usuario");
                    return Results.Problem(statusCode: 404, type: "https://httpstatuses.com/404", title: "No encontrado", detail: nf.Message);
                }
                catch (ConflictException cx)
                {
                    logger.LogWarning(cx, "Conflicto de negocio al crear usuario");
                    return Results.Problem(statusCode: 409, type: "https://httpstatuses.com/409", title: "Conflicto", detail: cx.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error inesperado al crear usuario");
                    return Results.Problem(statusCode: 500, title: "Error interno del servidor");
                }
            })
            .WithName("CrearUsuario")
            .WithOpenApi();
**/
        

#if DEMO_ENABLE_ADMIN
        group.MapPost("", async (CreateEventoCommand command, IUnitOfWork uow, IValidator<CreateEventoCommand> validator) =>
        {
            var handler = new CreateEventoHandler(uow, validator);
            var id = await handler.HandleAsync(command);
            return Results.Created($"/api/eventos/{id}", new { id });
        })
        .WithName("CreateEvento")
        .WithOpenApi();
#endif

#if DEMO_ENABLE_ADMIN
        // Entradas - Validación y Canje (se registran aquí para evitar tocar Program.cs)
        var entradasGroup = endpoints.MapGroup("entradas");

        entradasGroup.MapGet("/validar", async (string token, IConfiguration config, IUnitOfWork uow, CancellationToken ct) =>
        {
            var secret = config["ValidationTokens:Secret"];
            var validation = ValidationTokenHelper.ValidateToken(token, secret);
            if (validation.Status == TokenValidationStatus.InvalidToken)
                return Results.BadRequest(new { status = "invalid_token", detail = validation.Detail });
            if (validation.Status == TokenValidationStatus.Expired)
                return Results.BadRequest(new { status = "expired", detail = validation.Detail });

            var orderId = validation.OrderId!.Value;
            // Repositorio debe incluir Items
            var orden = await uow.Ordenes.GetByIdAsync(orderId, ct);
            if (orden is null)
                return Results.NotFound(new { status = "not_found" });

            if (orden.RedeemedAtUtc.HasValue)
                return Results.Conflict(new { status = "used" });

            // Validar estado de eventos asociados
            var eventoIds = orden.Items.Select(i => i.EventoId).Distinct().ToList();
            foreach (var eid in eventoIds)
            {
                var ev = await uow.Eventos.GetByIdAsync(eid, ct);
                var stockTotal = (ev?.Entradas?.Sum(x => (int?)x.StockDisponible) ?? 0);
                if (ev is null || ev.Fecha <= DateTime.UtcNow || stockTotal <= 0)
                    return Results.Conflict(new { status = "event_closed" });
            }

            var cantidadTotal = orden.Items.Sum(i => i.Cantidad);
            return Results.Ok(new
            {
                status = "ok",
                orderId = orden.Id,
                emailComprador = orden.EmailComprador,
                eventoIds,
                cantidadTotal
            });
        })
        .WithName("ValidarEntrada")
        .WithOpenApi();

        entradasGroup.MapPost("/canjear", async (HttpContext http, string token, IConfiguration config, IUnitOfWork uow, CancellationToken ct) =>
        {
            var secret = config["ValidationTokens:Secret"];
            var validation = ValidationTokenHelper.ValidateToken(token, secret);
            if (validation.Status == TokenValidationStatus.InvalidToken)
                return Results.BadRequest(new { status = "invalid_token", detail = validation.Detail });
            if (validation.Status == TokenValidationStatus.Expired)
                return Results.BadRequest(new { status = "expired", detail = validation.Detail });

            var orderId = validation.OrderId!.Value;
            var orden = await uow.Ordenes.GetByIdAsync(orderId, ct);
            if (orden is null)
                return Results.NotFound(new { status = "not_found" });

            if (orden.RedeemedAtUtc.HasValue)
                return Results.Conflict(new { status = "used" });

            // Validar estado de eventos asociados
            var eventoIds = orden.Items.Select(i => i.EventoId).Distinct().ToList();
            foreach (var eid in eventoIds)
            {
                var ev = await uow.Eventos.GetByIdAsync(eid, ct);
                var stockTotal = (ev?.Entradas?.Sum(x => (int?)x.StockDisponible) ?? 0);
                if (ev is null || ev.Fecha <= DateTime.UtcNow || stockTotal <= 0)
                    return Results.Conflict(new { status = "event_closed" });
            }

            // Marcar canje
            orden.RedeemedAtUtc = DateTime.UtcNow;
            if (http.Request.Headers.TryGetValue("X-Device-Id", out var deviceVals))
            {
                var dev = deviceVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(dev))
                    orden.RedeemedBy = dev;
            }

            uow.Ordenes.Update(orden);
            await uow.SaveChangesAsync(ct);

            return Results.Ok(new { status = "redeemed" });
        })
        .WithName("CanjearEntrada")
        .WithOpenApi();
#endif

        return endpoints;
    }
}
