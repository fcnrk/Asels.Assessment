using Asels.Assessment.Modules.Venues.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Venues.Infrastructure.Persistence;

public class VenuesDbContext(DbContextOptions<VenuesDbContext> options) : DbContext(options)
{
    public DbSet<Venue> Restaurants => Set<Venue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>()
            .HasKey(r => r.Id);
        
        var venueId = Guid.Parse("B1D7CA02-6804-43F9-BCDB-A1CCEC9971A6");

        modelBuilder.Entity<Venue>().HasData(
            new Venue() { Id = venueId, Name = "Test Restaurant", Address = "Test Address", Description = "Best in town", PhoneNumber = "1234567890", IsActive = true}
        );
    }
}