namespace Asels.Assessment.Modules.Orders.Contracts.Requests;

public record CreateOrderRequest(Guid VenueId, string Username, DateOnly OrderDate, IReadOnlyList<CreateOrderRequestItem> Items);