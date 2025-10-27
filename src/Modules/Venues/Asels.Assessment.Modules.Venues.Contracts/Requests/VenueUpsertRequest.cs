using System.ComponentModel.DataAnnotations;

namespace Asels.Assessment.Modules.Venues.Contracts.Requests;

public sealed record VenueUpsertRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(1000)] string? Description,
    [property: MaxLength(500)] string? Address,
    [property: Phone, MaxLength(50)] string? PhoneNumber,
    [property: Required] bool IsActive = true
);