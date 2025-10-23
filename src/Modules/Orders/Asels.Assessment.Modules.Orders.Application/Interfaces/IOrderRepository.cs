using Asels.Assessment.Modules.Orders.Domain.Entities;

namespace Asels.Assessment.Modules.Orders.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}