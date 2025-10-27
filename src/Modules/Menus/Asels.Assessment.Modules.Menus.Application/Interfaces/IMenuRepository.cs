using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Application.Interfaces;

public interface IMenuRepository
{
    Task<Menu?> GetActiveByVenueAndDayAsync(Guid venueId, DayOfWeek day, CancellationToken ct);
    Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Menu> AddAsync(Menu menu, CancellationToken ct);
    Task<Menu> UpdateAsync(Menu menu, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> DeactivateOtherMenusAsync(Guid venueId, DayOfWeek day, Guid excludeId, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}