using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Queries;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Menus.Api;

internal static class MenuEndpoints
{
    internal static void Map(IEndpointRouteBuilder ep)
    {
        var g = ep.MapGroup("/api/venues/{venueId:guid}/menus").WithTags("Menus");

        // GET all active menus for a given venue
        g.MapGet("/",
                async (Guid venueId, ISender sender, CancellationToken ct) =>
                {
                    var list = await sender.Send(new ListMenusByVenue.Query(venueId), ct);
                    return Results.Ok(list);
                })
            .WithName("Menus_List")
            .Produces<MenuDto>(200)
            .Produces(204);
        
        // GET active menu for a given day
        g.MapGet("/{day:int}",
                async (Guid venueId, int day, ISender sender, CancellationToken ct) =>
                {
                    var dto = await sender.Send(new GetMenuByDay.Query(venueId, (DayOfWeek)day), ct);
                    return dto is null ? Results.NoContent() : Results.Ok(dto);
                })
            .WithName("Menus_GetByDay")
            .Produces<MenuDto>(200)
            .Produces(204);

        // PUT
        g.MapPut("/{day:int}",
                async (Guid venueId, int day, MenuUpsertRequest req, ISender sender, CancellationToken ct) =>
                {
                    var dto = await sender.Send(
                        new UpsertMenu.Command(venueId, (DayOfWeek)day, req.ItemIds, req.Activate), ct);
                    return Results.Ok(dto);
                })
            .WithName("Menus_PutByDay")
            .Produces<MenuDto>(200)
            .ProducesProblem(400);

        // DELETE
        g.MapDelete("/{menuId:guid}",
                async (Guid menuId, ISender sender, CancellationToken ct) =>
                {
                    var ok = await sender.Send(new DeleteMenu.Command(menuId), ct);
                    return ok ? Results.NoContent() : Results.NotFound();
                })
            .WithName("Menus_Delete")
            .Produces(204)
            .Produces(404);
    }
}