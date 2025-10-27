using Asels.Assessment.Modules.Orders.Application.Interfaces;
using Asels.Assessment.Modules.Orders.Domain.Entities;
using Asels.Assessment.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Orders.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _db;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrdersDbContext db, ILogger<OrderRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Order> AddAsync(Order order, CancellationToken ct)
    {
        _logger.LogDebug($"[OrdersRepo] Adding new order for Customer={order.Username}, Venue={order.VenueId}");
        await _db.Orders.AddAsync(order, ct);
        return order;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}