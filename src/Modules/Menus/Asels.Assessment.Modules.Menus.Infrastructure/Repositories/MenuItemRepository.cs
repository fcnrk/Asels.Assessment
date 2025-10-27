using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Repositories;

public sealed class MenuItemRepository(MenusDbContext db) : IMenuItemRepository
{
    public Task<IReadOnlyList<MenuItem>> GetAllByVenueAsync(Guid venueId, CancellationToken ct)
        => db.MenuItems
            .Where(x => x.VenueId == venueId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<MenuItem>)t.Result, ct);

    public Task<List<MenuItem>> GetByIdsAsync(Guid restaurantId, IEnumerable<Guid> ids, CancellationToken ct)
        => db.MenuItems.Where(x => x.VenueId == restaurantId && ids.Contains(x.Id)).ToListAsync(ct);

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => db.MenuItems.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<MenuItem> AddAsync(MenuItem item, CancellationToken ct)
        => (await db.MenuItems.AddAsync(item, ct)).Entity;

    public Task<MenuItem> UpdateAsync(MenuItem item, CancellationToken ct)
        => Task.FromResult(db.MenuItems.Update(item).Entity);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var existing = await db.MenuItems.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (existing is null)
            return false;

        db.MenuItems.Remove(existing);
        return true;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}