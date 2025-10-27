using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Commands;

public static class PatchMenuItem
{
    public sealed record Command(
        Guid VenueId,
        Guid ItemId,
        string? Name,
        decimal? Price,
        string? Description,
        bool? IsAvailable
    ) : IRequest<MenuItemDto?>;

    public sealed class Handler(
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Command, MenuItemDto?>
    {
        public async Task<MenuItemDto?> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(PatchMenuItem)} for Item Id: {request.ItemId}");
            try
            {
                var entity = await items.GetByIdAsync(request.ItemId, ct);
                if (entity is null || entity.VenueId != request.VenueId)
                    throw new KeyNotFoundException($"Item {request.ItemId} not found for Venue {request.VenueId}.");

                if (request.Name is not null) entity.Name = request.Name;
                if (request.Price.HasValue) entity.Price = request.Price.Value;
                if (request.Description is not null) entity.Description = request.Description;
                if (request.IsAvailable.HasValue) entity.IsAvailable = request.IsAvailable.Value;

                entity = await items.UpdateAsync(entity, ct);
                await items.SaveChangesAsync(ct);
                logger.LogInformation($"Updated existing menu item {entity.Id}");

                return new MenuItemDto(entity.Id, entity.Name, entity.Price, entity.Description, entity.IsAvailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(PatchMenuItem)} for Menu Item Id: {request.ItemId}");
                throw;
            }
        }
    }
}