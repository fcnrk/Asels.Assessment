using Asels.Assessment.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>().HasKey(o => o.Id);
        b.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<OrderItem>().HasKey(oi => oi.Id);
    }
}