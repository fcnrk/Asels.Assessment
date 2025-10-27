using Asels.Assessment.Modules.Venues.Domain.Entities;

namespace Asels.Assessment.Modules.Venues.Application.Interfaces;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Venue> AddAsync(Venue venue, CancellationToken ct);
    Task<Venue> UpdateAsync(Venue venue, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}