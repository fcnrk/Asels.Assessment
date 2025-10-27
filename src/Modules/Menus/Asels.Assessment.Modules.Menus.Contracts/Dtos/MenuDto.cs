namespace Asels.Assessment.Modules.Menus.Contracts.Dtos;

public record MenuDto(
    Guid Id, 
    Guid VenueId, 
    DayOfWeek Day, 
    bool IsActive, 
    IReadOnlyList<MenuItemDto> Items);