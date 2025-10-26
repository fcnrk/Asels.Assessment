using Asels.Assessment.Modules.Venues.Domain.Entities;

namespace Asels.Assessment.Modules.Venues.Application.Interfaces;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Venue venue, CancellationToken ct);
    Task UpdateAsync(Venue venue, CancellationToken ct);
    Task DeleteAsync(Venue venue, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}