namespace Asels.Assessment.Modules.Orders.Contracts.Dtos;

public record OrderDto(Guid Id, Guid RestaurantId, Guid MenuId, string Day, string CustomerName, decimal TotalAmount, string Status, IReadOnlyList<OrderItemDto> Items);