using Asels.Assessment.Modules.Menu.Domain.Entities;

namespace Asels.Assessment.Modules.Menu.Application.Interfaces;

public interface IMenuRepository
{
    Task<Domain.Entities.Menu?> GetActiveByRestaurantAndDayAsync(Guid restaurantId, DayOfWeek day, CancellationToken ct);
    Task AddAsync(Domain.Entities.Menu menu, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Menu menu, CancellationToken ct);
    Task DeactivateOtherMenusAsync(Guid restaurantId, DayOfWeek day, Guid excludeId, CancellationToken ct);
    Task RemoveAsync(Domain.Entities.Menu menu, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}