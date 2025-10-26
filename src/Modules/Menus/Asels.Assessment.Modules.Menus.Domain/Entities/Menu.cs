namespace Asels.Assessment.Modules.Menus.Domain.Entities;

public sealed class Menu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RestaurantId { get; set; }
    public DayOfWeek Day { get; set; }
    public bool IsActive { get; set; } = true;
    public List<MenuEntry> Entries { get; set; } = new();
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
}