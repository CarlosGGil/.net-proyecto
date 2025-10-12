using System;
using System.Linq;
using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using Espectaculos.Application.Commands.CreateEvento;
using Espectaculos.Application.DTOs;
using Espectaculos.Application.Queries.GetEntradasPorEvento;
using Espectaculos.Application.Queries.GetEventoById;
using Espectaculos.Application.Queries.ListEventos;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Espectaculos.WebApi.Utils;

namespace Espectaculos.WebApi.Endpoints;

public static class EventosEndpoints
{
    public static IEndpointRouteBuilder MapEventosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("eventos");

        group.MapGet("",
                async (
                    IEventoRepository repo,
                    HttpContext ctx,
                    string? q = null,
                    string? sort = null,
                    string? dir = null,
                    int page = 1,
                    int pageSize = 10,
                    bool onlyPublished = false,
                    bool? disponible = null,
                    bool? onlyDisponibles = null) =>
                {
                    var handler = new ListEventosHandler(repo);
                    var effectiveOnlyDisponibles = (disponible ?? onlyDisponibles) ?? false;
                    var result = await handler.HandleAsync(new ListEventosQuery
                    {
                        Q = q,
                        Sort = sort,
                        Dir = dir,
                        Page = page,
                        PageSize = pageSize,
                        OnlyPublished = onlyPublished,
                        OnlyDisponibles = effectiveOnlyDisponibles
                    }, ctx.RequestAborted);

                    return Results.Ok(result); 
                })
            .WithName("ListEventos")
            .WithOpenApi(operation =>
            {
                var onlyDispParam = operation.Parameters.FirstOrDefault(p => string.Equals(p.Name, "onlyDisponibles", StringComparison.OrdinalIgnoreCase));
                if (onlyDispParam is not null)
                {
                    onlyDispParam.Deprecated = true;
                    var baseDesc = onlyDispParam.Description ?? string.Empty;
                    onlyDispParam.Description = string.IsNullOrWhiteSpace(baseDesc)
                        ? "Obsoleto. Use 'disponible'. Compatibilidad: true filtra disponibles; false no filtra."
                        : baseDesc + " Obsoleto. Use 'disponible'.";
                }

                var dispParam = operation.Parameters.FirstOrDefault(p => string.Equals(p.Name, "disponible", StringComparison.OrdinalIgnoreCase));
                if (dispParam is not null)
                {
                    var baseDesc = dispParam.Description ?? string.Empty;
                    dispParam.Description = string.IsNullOrWhiteSpace(baseDesc)
                        ? "Parámetro recomendado. true: solo eventos disponibles; false o ausencia: todos los eventos."
                        : baseDesc + " (Parámetro recomendado)";
                }

                return operation;
            });

        group.MapGet("/{id:guid}", async (Guid id, IUnitOfWork uow) =>
        {
            var handler = new GetEventoByIdHandler(uow);
            var dto = await handler.HandleAsync(new GetEventoByIdQuery(id));
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        })
        .WithName("GetEventoById")
        .WithOpenApi();

        group.MapGet("/{id:guid}/entradas", async (Guid id, IUnitOfWork uow) =>
        {
            var handler = new GetEntradasPorEventoHandler(uow);
            var list = await handler.HandleAsync(new GetEntradasPorEventoQuery(id));
            return Results.Ok(list);
        })
        .WithName("GetEntradasPorEvento")
        .WithOpenApi();

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
