<<<<<<< Updated upstream
using Espectaculos.Application.Commands.CreateUpdateBeneficio;
using Espectaculos.Application.Commands.RedeemBeneficio;
using Espectaculos.Application.Queries.ListBeneficios;
using Espectaculos.Application.DTOs;
using Espectaculos.Application.Abstractions;
using Espectaculos.Application.Abstractions.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
=======
using Espectaculos.Application.Commands.RedeemBeneficio;
using MediatR;
>>>>>>> Stashed changes

namespace Espectaculos.WebApi.Endpoints;

public static class BeneficiosEndpoints
{
<<<<<<< Updated upstream
    public static IEndpointRouteBuilder MapBeneficiosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("beneficios");

        group.MapGet("/", async (IBeneficioRepository repo) =>
        {
            var handler = new ListBeneficiosHandler(repo);
            var res = await handler.HandleAsync(new ListBeneficiosQuery());
            return Results.Ok(res);
        }).WithName("ListarBeneficios").WithOpenApi();

        // Crear BENEFICIO (POST) — omitir 'id' en el body
        group.MapPost("/", async (CreateUpdateBeneficioCommand cmd, IUnitOfWork uow, IValidator<CreateUpdateBeneficioCommand> validator, ILoggerFactory loggerFactory, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) =>
        {
            var logger = loggerFactory.CreateLogger("Beneficios");
            if (cmd.Id.HasValue)
                return Results.BadRequest(new { error = "Para crear no envíe 'id'. Use PUT /api/beneficios/{id} para actualizar." });

            var handler = new CreateUpdateBeneficioHandler(uow, validator);
            try
            {
                var id = await handler.HandleAsync(cmd);
                return Results.Created($"/api/beneficios/{id}", new { id });
            }
            catch (FluentValidation.ValidationException vex)
            {
                var errors = vex.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.BadRequest(new { errors });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbex)
            {
                logger.LogError(dbex, "Error de BD al crear beneficio");
                if (env.IsDevelopment())
                    return Results.Problem(detail: dbex.InnerException?.Message ?? dbex.Message, statusCode: 500, title: "Error interno al crear beneficio (BD)");
                return Results.Problem(statusCode: 500, title: "Error interno al crear beneficio");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al crear beneficio");
                if (env.IsDevelopment())
                    return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error interno al crear beneficio");
                return Results.Problem(statusCode: 500, title: "Error interno al crear beneficio");
            }
        }).WithName("CreateBeneficio").WithOpenApi();

        // Actualizar BENEFICIO (PUT) — id en ruta
    group.MapPut("/{id:guid}", async (Guid id, CreateUpdateBeneficioCommand body, IUnitOfWork uow, IValidator<CreateUpdateBeneficioCommand> validator, ILoggerFactory loggerFactory, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) =>
        {
            // Copiar body hacia un comando con Id explícito
            var cmd = new CreateUpdateBeneficioCommand
            {
                Id = id,
                Tipo = body.Tipo,
                Nombre = body.Nombre,
                Descripcion = body.Descripcion,
                VigenciaInicioUtc = body.VigenciaInicioUtc,
                VigenciaFinUtc = body.VigenciaFinUtc,
                CupoTotal = body.CupoTotal,
                CupoPorUsuario = body.CupoPorUsuario,
                RequiereBiometria = body.RequiereBiometria,
                CriterioElegibilidad = body.CriterioElegibilidad
            };

            var logger = loggerFactory.CreateLogger("Beneficios");
            var handler = new CreateUpdateBeneficioHandler(uow, validator);
            try
            {
                var updatedId = await handler.HandleAsync(cmd);
                return Results.Ok(new { id = updatedId });
            }
            catch (KeyNotFoundException knf)
            {
                return Results.NotFound(new { error = knf.Message });
            }
            catch (FluentValidation.ValidationException vex)
            {
                var errors = vex.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.BadRequest(new { errors });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbex)
            {
                logger.LogError(dbex, "Error de BD al actualizar beneficio {Id}", id);
                if (env.IsDevelopment())
                    return Results.Problem(detail: dbex.InnerException?.Message ?? dbex.Message, statusCode: 500, title: "Error interno al actualizar beneficio (BD)");
                return Results.Problem(statusCode: 500, title: "Error interno al actualizar beneficio");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar beneficio {Id}", id);
                if (env.IsDevelopment())
                    return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error interno al actualizar beneficio");
                return Results.Problem(statusCode: 500, title: "Error interno al actualizar beneficio");
            }
        }).WithName("UpdateBeneficio").WithOpenApi();

        group.MapPost("/redeem", async (RedeemBeneficioCommand cmd, IUnitOfWork uow, IValidator<RedeemBeneficioCommand> validator) =>
        {
            var handler = new RedeemBeneficioHandler(uow, validator);
            try
            {
                var id = await handler.HandleAsync(cmd);
                return Results.Ok(new { canjeId = id });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
        }).WithName("RedeemBeneficio").WithOpenApi();

        return endpoints;
    }
=======
    public static void MapBeneficiosEndpoints(this IEndpointRouteBuilder group)
    {
        var g = group.MapGroup("/beneficios");

        g.MapPost("/{id:guid}/redeem", async (Guid id, RedeemRequest req, IMediator mediator, CancellationToken ct) =>
        {
            var cmd = new RedeemBeneficioCommand(id, req.UsuarioId, req.VerificacionBiometrica, req.Firma);
            var ok = await mediator.Send(cmd, ct);
            return ok ? Results.NoContent() : Results.BadRequest();
        });
    }

    public record RedeemRequest(Guid UsuarioId, bool? VerificacionBiometrica, string? Firma);
>>>>>>> Stashed changes
}
