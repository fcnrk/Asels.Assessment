using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Repositories;

public sealed class MenuRepository(MenusDbContext db) : IMenuRepository
{
    public async Task<Menu?> GetActiveByVenueAndDayAsync(Guid venueId, DayOfWeek day, CancellationToken ct)
        => await db.Menus
            .Include(m => m.Entries)
            .ThenInclude(e => e.MenuItem)
            .FirstOrDefaultAsync(m => m.VenueId == venueId && m.Day == day && m.IsActive, ct);

    public Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct)
        => db.Menus.Include(m => m.Entries).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Menu> AddAsync(Menu menu, CancellationToken ct)
        => (await db.Menus.AddAsync(menu, ct)).Entity;

    public Task<Menu> UpdateAsync(Menu menu, CancellationToken ct)
        => Task.FromResult(db.Menus.Update(menu).Entity);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var existing = await db.Menus.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (existing is null) return false;
        db.Menus.Remove(existing);
        return true;
    }

    public async Task<bool> DeactivateOtherMenusAsync(Guid venueId, DayOfWeek day, Guid exceptMenuId, CancellationToken ct)
    {
        var others = await db.Menus
            .Where(m => m.VenueId == venueId && m.Day == day && m.Id != exceptMenuId && m.IsActive)
            .ToListAsync(ct);

        foreach (var o in others) o.IsActive = false;
        return true;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}