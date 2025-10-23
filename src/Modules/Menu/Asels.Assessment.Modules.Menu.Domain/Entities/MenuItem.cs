namespace Asels.Assessment.Modules.Menu.Domain.Entities;

public sealed class MenuItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MenuId { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;
}