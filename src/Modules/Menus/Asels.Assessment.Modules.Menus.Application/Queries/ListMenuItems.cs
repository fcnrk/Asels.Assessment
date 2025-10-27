using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Queries;

public static class ListMenuItems
{
    public sealed record Query(Guid VenueId) : IRequest<IReadOnlyList<MenuItemDto>>;

    public sealed class Handler(
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Query, IReadOnlyList<MenuItemDto>>
    {
        public async Task<IReadOnlyList<MenuItemDto>> Handle(Query request, CancellationToken ct)
        {
            var list = await items.GetAllByVenueAsync(request.VenueId, ct);

            return list
                .OrderBy(i => i.Name)
                .Select(i => new MenuItemDto(i.Id, i.Name, i.Price, i.Description, i.IsAvailable))
                .ToList();
        }
    }
}