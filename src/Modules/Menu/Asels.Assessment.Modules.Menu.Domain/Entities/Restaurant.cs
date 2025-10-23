namespace Asels.Assessment.Modules.Menu.Domain.Entities;

public sealed class Restaurant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public List<Menu> Menus { get; set; } = new();
}