using Microsoft.EntityFrameworkCore;
using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
public class MenusDbContext(DbContextOptions<MenusDbContext> options) : DbContext(options)
{
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuEntry> MenuEntries => Set<MenuEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Menu>()
            .HasIndex(m => new { m.RestaurantId, m.Day, m.IsActive });
        
        b.Entity<MenuEntry>()
            .HasOne(me => me.MenuItem)
            .WithMany()
            .HasForeignKey(me => me.MenuItemId);

        b.Entity<MenuEntry>()
            .HasOne<Menu>()
            .WithMany(m => m.Entries)
            .HasForeignKey(me => me.MenuId);

        var restaurantId = Guid.Parse("B1D7CA02-6804-43F9-BCDB-A1CCEC9971A6");
        var weekday = DayOfWeek.Monday;
        var weekend = DayOfWeek.Saturday;
        var menu1Id = Guid.NewGuid();
        var menu2Id = Guid.NewGuid();
        var menuItemIds = new List<Guid>();
        for (int i = 0; i < 8; i++)
            menuItemIds.Add(Guid.NewGuid());

        b.Entity<Menu>().HasData(
            new Menu { Id = menu1Id, RestaurantId = restaurantId, Day = weekday, IsActive = true, CreatedUtc = DateTime.UtcNow },
            new Menu { Id = menu2Id, RestaurantId = restaurantId, Day = weekend, IsActive = true, CreatedUtc = DateTime.UtcNow }
        );
        b.Entity<MenuItem>().HasData(
            new MenuItem { Id = menuItemIds[0], RestaurantId = restaurantId, Name = "Test Item 1", Price = 220 },
            new MenuItem { Id = menuItemIds[1], RestaurantId = restaurantId, Name = "Test Item 2", Price = 150 },
            new MenuItem { Id = menuItemIds[2], RestaurantId = restaurantId, Name = "Test Item 3", Price = 180 },
            new MenuItem { Id = menuItemIds[3], RestaurantId = restaurantId, Name = "Test Item 4", Price = 240 },
            new MenuItem { Id = menuItemIds[4], RestaurantId = restaurantId, Name = "Test Item 5", Price = 90 },
            new MenuItem { Id = menuItemIds[5], RestaurantId = restaurantId, Name = "Test Item 6", Price = 130 },
            new MenuItem { Id = menuItemIds[6], RestaurantId = restaurantId, Name = "Test Item 7", Price = 122 },
            new MenuItem { Id = menuItemIds[7], RestaurantId = restaurantId, Name = "Test Item 8", Price = 60 }
        );
        b.Entity<MenuEntry>().HasData(
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu1Id, MenuItemId = menuItemIds[0] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu1Id, MenuItemId = menuItemIds[1] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu1Id, MenuItemId = menuItemIds[2] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu1Id, MenuItemId = menuItemIds[3] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu2Id, MenuItemId = menuItemIds[4] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu2Id, MenuItemId = menuItemIds[5] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu2Id, MenuItemId = menuItemIds[6] },
            new MenuEntry { Id = Guid.NewGuid(), MenuId = menu2Id, MenuItemId = menuItemIds[7] }
        );
    }
}