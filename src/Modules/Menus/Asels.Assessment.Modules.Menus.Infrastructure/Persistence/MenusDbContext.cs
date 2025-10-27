using Microsoft.EntityFrameworkCore;
using Asels.Assessment.Modules.Menus.Domain.Entities;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
public class MenusDbContext(DbContextOptions<MenusDbContext> options) : DbContext(options)
{
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuEntry> MenuEntries => Set<MenuEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.VenueId).IsRequired();
                entity.Property(m => m.Day).IsRequired();
                entity.Property(m => m.IsActive).IsRequired();
                entity.Property(m => m.UpdatedUtc);
                entity.HasMany(m => m.Entries)
                    .WithOne()
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder.Entity<MenuEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MenuItemId).IsRequired();
            entity.HasOne(me => me.MenuItem)
                .WithMany()
                .HasForeignKey(me => me.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
            

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.VenueId).IsRequired();
            entity.Property(i => i.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(i => i.Price)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(i => i.Description).HasMaxLength(500);
            entity.Property(i => i.IsAvailable).IsRequired();
        });

        var restaurantId = Guid.Parse("B1D7CA02-6804-43F9-BCDB-A1CCEC9971A6");
        var weekday = DayOfWeek.Monday;
        var weekend = DayOfWeek.Saturday;
        var menu1Id = Guid.NewGuid();
        var menu2Id = Guid.NewGuid();
        var menuItemIds = new List<Guid>();
        for (int i = 0; i < 8; i++)
            menuItemIds.Add(Guid.NewGuid());

        modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = menu1Id, VenueId = restaurantId, Day = weekday, IsActive = true, CreatedUtc = DateTime.UtcNow },
            new Menu { Id = menu2Id, VenueId = restaurantId, Day = weekend, IsActive = true, CreatedUtc = DateTime.UtcNow }
        );
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = menuItemIds[0], VenueId = restaurantId, Name = "Test Item 1", Price = 220 },
            new MenuItem { Id = menuItemIds[1], VenueId = restaurantId, Name = "Test Item 2", Price = 150 },
            new MenuItem { Id = menuItemIds[2], VenueId = restaurantId, Name = "Test Item 3", Price = 180 },
            new MenuItem { Id = menuItemIds[3], VenueId = restaurantId, Name = "Test Item 4", Price = 240 },
            new MenuItem { Id = menuItemIds[4], VenueId = restaurantId, Name = "Test Item 5", Price = 90 },
            new MenuItem { Id = menuItemIds[5], VenueId = restaurantId, Name = "Test Item 6", Price = 130 },
            new MenuItem { Id = menuItemIds[6], VenueId = restaurantId, Name = "Test Item 7", Price = 122 },
            new MenuItem { Id = menuItemIds[7], VenueId = restaurantId, Name = "Test Item 8", Price = 60 }
        );
        modelBuilder.Entity<MenuEntry>().HasData(
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