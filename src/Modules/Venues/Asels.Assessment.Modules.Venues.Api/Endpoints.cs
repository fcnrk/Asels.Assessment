using Asels.Assessment.Modules.Venues.Application.Commands;
using Asels.Assessment.Modules.Venues.Application.Queries;
using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using Asels.Assessment.Modules.Venues.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Venues.Api;

internal static class Endpoints
{
    internal static void Map(IEndpointRouteBuilder ep)
    {
        var g = ep.MapGroup("/api/venues").WithTags("Venues");

        // CREATE
        g.MapPost("/",
                async (VenueUpsertRequest req, ISender sender, CancellationToken ct) =>
                {
                    var cmd = new UpsertVenue.Command(null, req.Name, req.Description, req.Address, req.PhoneNumber,
                        req.IsActive);
                    var dto = await sender.Send(cmd, ct);
                    return Results.Created($"/api/venues/{dto.Id}", dto);
                })
            .WithName("Venues_Create")
            .Produces<VenueDto>(201)
            .ProducesProblem(400);

        // READ ALL
        g.MapGet("/",
                async (ISender sender, CancellationToken ct) =>
                {
                    var list = await sender.Send(new GetAllVenues.Query(), ct);
                    return Results.Ok(list);
                })
            .WithName("Venues_List")
            .Produces<IReadOnlyList<VenueDto>>(200);

        // READ ONE
        g.MapGet("/{id:guid}",
                async (Guid id, ISender sender, CancellationToken ct) =>
                {
                    var dto = await sender.Send(new GetVenueById.Query(id), ct);
                    return dto is null ? Results.NotFound() : Results.Ok(dto);
                })
            .WithName("Venues_GetById")
            .Produces<VenueDto>(200)
            .Produces(404);

        // UPDATE (full, idempotent)
        g.MapPut("/{id:guid}",
                async (Guid id, VenueUpsertRequest req, ISender sender, CancellationToken ct) =>
                {
                    var cmd = new UpsertVenue.Command(id, req.Name, req.Description, req.Address, req.PhoneNumber,
                        req.IsActive);
                    var dto = await sender.Send(cmd, ct);
                    return Results.Ok(dto);
                })
            .WithName("Venues_Put")
            .Produces<VenueDto>(200)
            .ProducesProblem(400);

        // PARTIAL UPDATE — PATCH 
        g.MapPatch("/{id:guid}",
                async (Guid id, VenuePatchRequest req, ISender sender, CancellationToken ct) =>
                {
                    if (req.IsActive is null)
                        return Results.BadRequest(new
                            { error = "Provide at least one field to patch (e.g., isActive)." });

                    var dto = await sender.Send(new SetVenueActiveState.Command(id, req.IsActive.Value), ct);
                    return Results.Ok(dto);
                })
            .WithName("Venues_Patch")
            .Produces<VenueDto>(200)
            .ProducesProblem(400)
            .Produces(404);

        // DELETE 
        g.MapDelete("/{id:guid}",
                async (Guid id, ISender sender, CancellationToken ct) =>
                {
                    var deleted = await sender.Send(new DeleteVenue.Command(id), ct); // bool
                    return deleted ? Results.NoContent() : Results.NotFound();
                })
            .WithName("Venues_Delete")
            .Produces(204)
            .Produces(404);
    }
}