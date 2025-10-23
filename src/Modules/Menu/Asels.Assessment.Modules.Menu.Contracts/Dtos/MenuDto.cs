namespace Asels.Assessment.Modules.Menu.Contracts.Dtos;

public record MenuDto(Guid Id, Guid RestaurantId, DayOfWeek Day, bool IsActive, IReadOnlyList<MenuItemDto> Items);