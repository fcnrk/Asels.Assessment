using Asels.Assessment.Modules.Orders.Contracts.Dtos;

namespace Asels.Assessment.Modules.Orders.Contracts.ReadApis;

public interface IOrderReadService
{
    Task<OrderDto?> GetByIdAsync(Guid orderId, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct);
}