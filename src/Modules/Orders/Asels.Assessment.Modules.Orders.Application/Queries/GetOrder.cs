using System.Data;
using Asels.Assessment.Modules.Orders.Application.Interfaces;
using Asels.Assessment.Modules.Orders.Contracts.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Orders.Application.Queries;

public static class GetOrder
{
    public sealed record Query(Guid OrderId) : IRequest<OrderDto?>;

    // Handler implementation
    public sealed class Handler(
        IOrderRepository orders,
        ILogger<Handler> logger
    ) : IRequestHandler<Query, OrderDto?>
    {
        public async Task<OrderDto?> Handle(Query request, CancellationToken ct)
        {
            logger.LogInformation("[Orders] GetOrder (OrderId={OrderId})", request.OrderId);

            var order = await orders.GetByIdAsync(request.OrderId, ct);
            if (order is null)
            {
                logger.LogWarning("[Orders] Order not found (OrderId={OrderId})", request.OrderId);
                return null;
            }

            var dto = new OrderDto(
                order.Id,
                order.VenueId,
                order.OrderDate,
                order.Username,
                order.Total,
                order.Status.ToString(),
                order.Items.Select(i =>
                    new OrderItemDto(i.MenuItemId, i.MenuItemName, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)
                ).ToList()
            );

            logger.LogInformation($"[Orders] GetOrder success (OrderId={order.Id}, Total={order.Total})");

            return dto;
        }
    }
}