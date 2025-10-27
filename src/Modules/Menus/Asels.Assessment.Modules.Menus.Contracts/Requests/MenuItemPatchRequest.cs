using System.ComponentModel.DataAnnotations;

namespace Asels.Assessment.Modules.Menus.Contracts.Requests;

public sealed record MenuItemPatchRequest(
    [property: MaxLength(200)] string? Name,
    [property: Range(0, double.MaxValue)] decimal? Price,
    [property: MaxLength(500)] string? Description,
    bool? IsAvailable
);