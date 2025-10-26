namespace Asels.Assessment.Modules.Menus.Domain.Entities;

public sealed class MenuEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MenuId { get; set; }
    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
}