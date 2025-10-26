using Asels.Assessment.Modules.Venues.Contracts.Dtos;

namespace Asels.Assessment.Modules.Venues.Contracts.ReadApis;

public interface IVenueReadService
{
    Task<VenueDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<VenueDto>> GetAllAsync(CancellationToken ct);
}