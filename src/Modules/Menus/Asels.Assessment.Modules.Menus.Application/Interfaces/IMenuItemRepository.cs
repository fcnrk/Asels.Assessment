using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Application.Interfaces;

public interface IMenuItemRepository
{
    Task<IReadOnlyList<MenuItem>> GetAllByVenueAsync(Guid venueId, CancellationToken ct);
    Task<List<MenuItem>> GetByIdsAsync(Guid venueId, IEnumerable<Guid> ids, CancellationToken ct);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<MenuItem> AddAsync(MenuItem item, CancellationToken ct);
    Task<MenuItem> UpdateAsync(MenuItem item, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}