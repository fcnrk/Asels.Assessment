namespace Asels.Assessment.Modules.Orders.Contracts.Dtos;

public record OrderDto(Guid Id, Guid VenueId, DateOnly Day, string Username, decimal TotalAmount, string Status, IReadOnlyList<OrderItemDto> Items);