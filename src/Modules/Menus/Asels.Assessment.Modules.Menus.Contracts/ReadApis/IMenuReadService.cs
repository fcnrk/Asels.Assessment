using Asels.Assessment.Modules.Menus.Contracts.Dtos;

namespace Asels.Assessment.Modules.Menus.Contracts.ReadApis;

public interface IMenuReadService
{
    Task<MenuDto?> GetActiveByRestaurantAndDayAsync(Guid restaurantId, DayOfWeek day, CancellationToken ct);
    Task<IReadOnlyList<MenuDto>> GetAllByRestaurantAsync(Guid restaurantId, CancellationToken ct);
}