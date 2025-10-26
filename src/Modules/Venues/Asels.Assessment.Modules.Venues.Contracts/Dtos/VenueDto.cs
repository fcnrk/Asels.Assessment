namespace Asels.Assessment.Modules.Venues.Contracts.Dtos;

public record VenueDto(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    string? PhoneNumber,
    bool IsActive
);