using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Application.Interfaces;

public interface IMenuRepository
{
    Task<Menu?> GetActiveByRestaurantAndDayAsync(Guid restaurantId, DayOfWeek day, CancellationToken ct);
    Task AddAsync(Menu menu, CancellationToken ct);
    Task UpdateAsync(Menu menu, CancellationToken ct);
    Task DeactivateOtherMenusAsync(Guid restaurantId, DayOfWeek day, Guid excludeId, CancellationToken ct);
    Task RemoveAsync(Menu menu, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}