using Asels.Assessment.Modules.Menu.Contracts.Dtos;

namespace Asels.Assessment.Modules.Menu.Contracts.ReadApis;

public interface IMenuReadService
{
    Task<MenuDto?> GetActiveByRestaurantAndDayAsync(Guid restaurantId, DayOfWeek day, CancellationToken ct);
    Task<IReadOnlyList<MenuDto>> GetAllByRestaurantAsync(Guid restaurantId, CancellationToken ct);
}