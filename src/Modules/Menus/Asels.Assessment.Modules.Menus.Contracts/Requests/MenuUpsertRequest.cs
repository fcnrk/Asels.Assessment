using System.ComponentModel.DataAnnotations;

namespace Asels.Assessment.Modules.Menus.Contracts.Requests;

public sealed record MenuUpsertRequest(
    [property: MinLength(0)] IReadOnlyList<Guid> ItemIds,
    bool Activate = true
);