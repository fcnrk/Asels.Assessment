namespace Asels.Assessment.Modules.Orders.Contracts.Requests;

public record CreateOrderRequestItem(Guid MenuItemId, int Quantity);