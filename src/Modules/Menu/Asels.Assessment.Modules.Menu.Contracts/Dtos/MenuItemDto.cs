namespace Asels.Assessment.Modules.Menu.Contracts.Dtos;

public record MenuItemDto(Guid Id, string Name, decimal Price, string? Description, bool IsAvailable);