using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Services;

public sealed class MenuReadService(MenusDbContext db) : IMenuReadService
{
    public async Task<MenuDto?> GetActiveByVenueAndDayAsync(Guid venueId, DayOfWeek day, CancellationToken ct)
    {
        var menu = await db.Menus
            .AsNoTracking()
            .Include(m => m.Entries)
            .ThenInclude(e => e.MenuItem)
            .FirstOrDefaultAsync(m => m.VenueId == venueId && m.Day == day && m.IsActive, ct);

        if (menu is null)
            return null;

        var dto = new MenuDto(
            menu.Id,
            menu.VenueId,
            menu.Day,
            menu.IsActive,
            menu.Entries
                .Where(e => e.MenuItem != null)
                .Select(e => new MenuItemDto(
                    e.MenuItem!.Id,
                    e.MenuItem.Name,
                    e.MenuItem.Price,
                    e.MenuItem.Description,
                    e.MenuItem.IsAvailable
                ))
                .ToList()
        );

        return dto;
    }

    public async Task<IReadOnlyList<MenuDto>> GetAllByVenueAsync(Guid venueId, CancellationToken ct)
    {
        var menus = await db.Menus
            .AsNoTracking()
            .Include(m => m.Entries)
            .ThenInclude(e => e.MenuItem)
            .Where(m => m.VenueId == venueId)
            .OrderBy(m => m.Day)
            .ToListAsync(ct);

        var result = menus.Select(m => new MenuDto(
            m.Id,
            m.VenueId,
            m.Day,
            m.IsActive,
            m.Entries
                .Where(e => e.MenuItem != null)
                .Select(e => new MenuItemDto(
                    e.MenuItem!.Id,
                    e.MenuItem.Name,
                    e.MenuItem.Price,
                    e.MenuItem.Description,
                    e.MenuItem.IsAvailable
                ))
                .ToList()
        )).ToList();

        return result;
    }
}