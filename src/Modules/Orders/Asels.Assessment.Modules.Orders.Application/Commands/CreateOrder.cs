using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using Asels.Assessment.Modules.Orders.Application.Interfaces;
using Asels.Assessment.Modules.Orders.Contracts.Dtos;
using Asels.Assessment.Modules.Orders.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Orders.Application.Commands;

public static class CreateOrder
{
    public sealed record Command(
        Guid VenueId,
        DateOnly OrderDate,
        string Username,
        IReadOnlyDictionary<Guid, int> MenuItemIdsAndQuantities
    ) : IRequest<OrderDto>;

    public sealed class Handler(
        IMenuReadService menus,
        IOrderRepository orders,
        ILogger<Handler> logger
    ) : IRequestHandler<Command, OrderDto>
    {
        public async Task<OrderDto> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"[Orders] CreateOrder (Venue={request.VenueId}, Date={request.OrderDate}, Items={request.MenuItemIdsAndQuantities?.Count ?? 0})");

            try
            {
                if (request.VenueId == Guid.Empty)
                    throw new ArgumentException("VenueId is required.");
                if (request.MenuItemIdsAndQuantities is null || request.MenuItemIdsAndQuantities.Count == 0)
                    throw new ArgumentException("At least one menu item must be provided.");

                var day = request.OrderDate.ToDateTime(TimeOnly.MinValue).DayOfWeek;

                var menu = await menus.GetActiveByVenueAndDayAsync(request.VenueId, day, ct);
                if (menu is null || !menu.IsActive)
                    throw new InvalidOperationException("No active menu found for the venue on the requested day.");

                var allowedItems = menu.Items
                    .Where(i => i.IsAvailable)
                    .ToDictionary(i => i.Id, i => i);

                var invalid = request.MenuItemIdsAndQuantities
                    .Where(id => !allowedItems.ContainsKey(id.Key))
                    .Select(id => id.Key)
                    .Distinct()
                    .ToList();

                if (invalid.Count > 0)
                    throw new InvalidOperationException($"One or more menu items are not in the active menu for that day. ({string.Join(", ", invalid)})");

                var order = new Order
                {
                    VenueId = request.VenueId,
                    OrderDate = request.OrderDate
                };

                foreach (var id in request.MenuItemIdsAndQuantities)
                {
                    var item = allowedItems[id.Key];
                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = item.Id,
                        MenuItemName = item.Name,
                        UnitPrice = item.Price,
                        Quantity = id.Value
                    });
                }

                order = await orders.AddAsync(order, ct);
                await orders.SaveChangesAsync(ct);

                var dto = new OrderDto(
                    order.Id,
                    order.VenueId,
                    order.OrderDate,
                    request.Username,
                    order.Total,
                    order.Status.ToString(),
                    order.Items.Select(i => new OrderItemDto(i.MenuItemId, i.MenuItemName, i.Quantity, i.UnitPrice, i.Quantity*i.UnitPrice)).ToList()
                );

                logger.LogInformation($"[Orders] CreateOrder completed (OrderId={order.Id}, Total={order.Total})");

                return dto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(CreateOrder)} for (Venue={request.VenueId}, Date={request.OrderDate}).");
                throw;
            }
        }
    }
}