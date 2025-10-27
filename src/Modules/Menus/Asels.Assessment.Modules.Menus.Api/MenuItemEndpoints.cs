using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Queries;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Menus.Api;

internal static class MenuItemEndpoints
{
    internal static void Map(IEndpointRouteBuilder ep)
    {
        var g = ep.MapGroup("/api/venues/{venueId:guid}/menu-items").WithTags("MenuItems");

        // LIST all items for a venue
        g.MapGet("/",
            async (Guid venueId, ISender sender, CancellationToken ct) =>
                Results.Ok(await sender.Send(new ListMenuItems.Query(venueId), ct)))
         .WithName("MenuItems_List")
         .Produces<IReadOnlyList<MenuItemDto>>(200);

        // GET one item by id
        g.MapGet("/{itemId:guid}",
            async (Guid venueId, Guid itemId, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(new GetMenuItemById.Query(venueId, itemId), ct);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
         .WithName("MenuItems_GetById")
         .Produces<MenuItemDto>(200)
         .Produces(404);

        // POST create
        g.MapPost("/",
            async (Guid venueId, MenuItemUpsertRequest req, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(
                    new UpsertMenuItem.Command(venueId, null, req.Name, req.Price, req.Description, req.IsAvailable), ct);
                return Results.Created($"/api/venues/{venueId}/menu-items/{dto.Id}", dto);
            })
         .WithName("MenuItems_Create")
         .Produces<MenuItemDto>(201)
         .ProducesProblem(400);

        // PUT full update (idempotent)
        g.MapPut("/{itemId:guid}",
            async (Guid venueId, Guid itemId, MenuItemUpsertRequest req, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(
                    new UpsertMenuItem.Command(venueId, itemId, req.Name, req.Price, req.Description, req.IsAvailable), ct);
                return Results.Ok(dto);
            })
         .WithName("MenuItems_Update")
         .Produces<MenuItemDto>(200)
         .ProducesProblem(400)
         .Produces(404);

        // PATCH partial update
        g.MapPatch("/{itemId:guid}",
            async (Guid venueId, Guid itemId, MenuItemPatchRequest req, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(
                    new PatchMenuItem.Command(venueId, itemId, req.Name, req.Price, req.Description, req.IsAvailable), ct);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
         .WithName("MenuItems_Patch")
         .Produces<MenuItemDto>(200)
         .Produces(404);

        // DELETE
        g.MapDelete("/{itemId:guid}",
            async (Guid venueId, Guid itemId, ISender sender, CancellationToken ct) =>
            {
                var ok = await sender.Send(new DeleteMenuItem.Command(venueId, itemId), ct);
                return ok ? Results.NoContent() : Results.NotFound();
            })
         .WithName("MenuItems_Delete")
         .Produces(204)
         .Produces(404);
    }
}