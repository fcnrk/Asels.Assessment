using Asels.Assessment.Modules.Menus.Contracts.Dtos;

namespace Asels.Assessment.Modules.Menus.Contracts.ReadApis;

public interface IMenuReadService
{
    Task<MenuDto?> GetActiveByVenueAndDayAsync(Guid venueId, DayOfWeek day, CancellationToken ct);
    Task<IReadOnlyList<MenuDto>> GetAllByVenueAsync(Guid venueId, CancellationToken ct);
}