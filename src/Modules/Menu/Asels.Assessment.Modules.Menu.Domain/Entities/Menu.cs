namespace Asels.Assessment.Modules.Menu.Domain.Entities;

public sealed class Menu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RestaurantId { get; set; }
    public DayOfWeek Day { get; set; }
    public bool IsActive { get; set; } = true;
    public List<MenuItem> Items { get; set; } = new();
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
}