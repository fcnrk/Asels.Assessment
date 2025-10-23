namespace Asels.Assessment.Modules.Orders.Contracts.Dtos;

public record OrderItemDto(Guid MenuItemId, string Name, int Quantity, decimal UnitPrice, decimal LineTotal);