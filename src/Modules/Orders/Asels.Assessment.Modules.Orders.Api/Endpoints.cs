using Asels.Assessment.Modules.Orders.Application.Commands;
using Asels.Assessment.Modules.Orders.Application.Queries;
using Asels.Assessment.Modules.Orders.Contracts.Dtos;
using Asels.Assessment.Modules.Orders.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Orders.Api;

public static class Endpoints
{
    public static void Map(IEndpointRouteBuilder ep)
    {
        var customer = ep.MapGroup("/api/orders")
                         .WithTags("Orders");
        var venue = ep.MapGroup("/api/venues/{venueId:guid}/orders")
            .WithTags("Orders:Venue");

        // Create an order
        venue.MapPost("/",
            async (Guid venueId, CreateOrderRequest req, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(new CreateOrder.Command(
                    venueId,
                    req.OrderDate,
                    req.Username,
                    req.Items.ToDictionary(x => x.MenuItemId, x => x.Quantity)
                ), ct);

                return Results.Created($"/api/orders/{dto.Id}", dto);
            })
            .WithTags("Orders")
            .WithName("Orders_Create")
            .Produces<OrderDto>(201)
            .ProducesProblem(400);


        customer.MapGet("/{orderId:guid}",
            async (Guid venueId, Guid orderId, ISender sender, CancellationToken ct) =>
            {
                var dto = await sender.Send(new GetOrder.Query(orderId), ct);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("Orders_GetById")
            .Produces<OrderDto>(200)
            .Produces(404);

    }
}