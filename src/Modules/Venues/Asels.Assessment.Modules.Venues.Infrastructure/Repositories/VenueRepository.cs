using Asels.Assessment.Modules.Venues.Application.Interfaces;
using Asels.Assessment.Modules.Venues.Domain.Entities;
using Asels.Assessment.Modules.Venues.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Venues.Infrastructure.Repositories;

public sealed class VenueRepository : IVenueRepository
{
    private readonly VenuesDbContext _dbContext;
    private readonly DbSet<Venue> _venues;

    public VenueRepository(VenuesDbContext dbContext)
    {
        _dbContext = dbContext;
        _venues = _dbContext.Venues;
    }

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _venues.FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<Venue> AddAsync(Venue venue, CancellationToken ct)
    {
        var entry = await _venues.AddAsync(venue, ct);
        return entry.Entity;
    }

    public Task<Venue> UpdateAsync(Venue venue, CancellationToken ct)
    {
        var entry = _venues.Update(venue);
        return Task.FromResult(entry.Entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var existing = await _venues.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (existing is null)
            return false;

        _venues.Remove(existing);
        return true;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}