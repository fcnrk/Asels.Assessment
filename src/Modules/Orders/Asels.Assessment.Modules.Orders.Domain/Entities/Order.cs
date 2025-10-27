using Asels.Assessment.Modules.Orders.Domain.Enums;

namespace Asels.Assessment.Modules.Orders.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VenueId { get; set; }
    public string? Username { get; set; }
    public DateOnly OrderDate { get; set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}