using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using Asels.Assessment.Modules.Venues.Contracts.ReadApis;
using Asels.Assessment.Modules.Venues.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Asels.Assessment.Modules.Venues.Infrastructure.Services;

public sealed class VenueReadService(VenuesDbContext dbContext) : IVenueReadService
{
    public async Task<VenueDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Venues
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new VenueDto(
                r.Id,
                r.Name,
                r.Description,
                r.Address,
                r.PhoneNumber,
                r.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<VenueDto>> GetAllAsync(CancellationToken ct)
        => await dbContext.Venues
            .AsNoTracking()
            .Select(r => new VenueDto(
                r.Id,
                r.Name,
                r.Description,
                r.Address,
                r.PhoneNumber,
                r.IsActive))
            .ToListAsync(ct);
}