using Asels.Assessment.Modules.Orders.Domain.Enums;

namespace Asels.Assessment.Modules.Orders.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RestaurantId { get; set; }
    public Guid MenuId { get; set; }
    public string Day { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public Guid CreatedByUserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}