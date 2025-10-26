using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Application.Interfaces;

public interface IMenuItemRepository
{
    Task<List<MenuItem>> GetByIdsAsync(Guid restaurantId, IEnumerable<Guid> ids, CancellationToken ct);
    Task AddAsync(MenuItem item, CancellationToken ct);
    Task UpdateAsync(MenuItem item, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}