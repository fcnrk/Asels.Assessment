using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Queries;

public static class GetMenuItemById
{
    public sealed record Query(Guid VenueId, Guid ItemId) : IRequest<MenuItemDto?>;

    public sealed class Handler(
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Query, MenuItemDto?>
    {
        public async Task<MenuItemDto?> Handle(Query request, CancellationToken ct)
        {
            var entity = await items.GetByIdAsync(request.ItemId, ct);
            if (entity is null || entity.VenueId != request.VenueId)
            {
                logger.LogWarning($"[MenuItems] GetById: Item {request.ItemId} not found for Venue {request.VenueId}");
                return null;
            }

            return new MenuItemDto(entity.Id, entity.Name, entity.Price, entity.Description, entity.IsAvailable);
        }
    }
}