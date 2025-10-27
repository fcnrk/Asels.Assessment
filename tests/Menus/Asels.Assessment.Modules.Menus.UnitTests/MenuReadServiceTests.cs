using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Asels.Assessment.Modules.Menus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class MenuReadServiceTests
{
    private static MenusDbContext NewCtx()
    {
        var opts = new DbContextOptionsBuilder<MenusDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new MenusDbContext(opts);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task GetByRestaurantAndDayAsync_Returns_Active_Menu_With_Items()
    {
        using var db = NewCtx();
        var venueId = Guid.NewGuid();

        var item1 = new MenuItem { VenueId = venueId, Name = "A", Price = 5 };
        var item2 = new MenuItem { VenueId = venueId, Name = "B", Price = 6 };
        db.MenuItems.AddRange(item1, item2);

        var menu = new Menu { VenueId = venueId, Day = DayOfWeek.Wednesday, IsActive = true };
        menu.Entries.Add(new MenuEntry { MenuItemId = item1.Id, MenuItem = item1 });
        menu.Entries.Add(new MenuEntry { MenuItemId = item2.Id, MenuItem = item2 });
        db.Menus.Add(menu);

        await db.SaveChangesAsync();

        var svc = new MenuReadService(db);
        var dto = await svc.GetActiveByVenueAndDayAsync(venueId, DayOfWeek.Wednesday, CancellationToken.None);

        dto.Should().NotBeNull();
        dto!.Items.Should().HaveCount(2);
        dto.Items.Select(i => i.Name).Should().Contain(new[] { "A", "B" });
    }
}