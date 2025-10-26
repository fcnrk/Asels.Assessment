using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Repositories;

public sealed class MenuRepository(MenusDbContext db) : IMenuRepository
{
    public async Task<Domain.Entities.Menu?> GetActiveByRestaurantAndDayAsync(Guid restaurantId, DayOfWeek day, CancellationToken ct)
    {
        return await db.Menus
            .Include(m => m.Entries)
            .ThenInclude(e => e.MenuItem)
            .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId && m.Day == day && m.IsActive, ct);
    }

    public async Task AddAsync(Domain.Entities.Menu menu, CancellationToken ct)
    {
        await db.Menus.AddAsync(menu, ct);
    }

    public Task UpdateAsync(Domain.Entities.Menu menu, CancellationToken ct)
    {
        db.Menus.Update(menu);
        return Task.CompletedTask;
    }

    public async Task DeactivateOtherMenusAsync(Guid restaurantId, DayOfWeek day, Guid excludeMenuId, CancellationToken ct)
    {
        var others = await db.Menus
            .Where(m => m.RestaurantId == restaurantId && m.Day == day && m.Id != excludeMenuId && m.IsActive)
            .ToListAsync(ct);

        foreach (var o in others)
            o.IsActive = false;
    }

    public Task RemoveAsync(Domain.Entities.Menu menu, CancellationToken ct)
    {
        db.Menus.Remove(menu);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}