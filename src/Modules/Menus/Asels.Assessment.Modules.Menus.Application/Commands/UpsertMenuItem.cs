using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Commands;

public static class UpsertMenuItem
{
    public sealed record Command(
        Guid VenueId,
        Guid? ItemId,
        string Name,
        decimal Price,
        string? Description,
        bool IsAvailable = true
    ) : IRequest<MenuItemDto>;

    public sealed class Handler(
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Command, MenuItemDto>
    {
        public async Task<MenuItemDto> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"[MenuItems] Upsert (Venue {request.VenueId}, Item {request.ItemId})");

            MenuItem? entity = null;

            try
            {
                if (request.ItemId.HasValue)
                    entity = await items.GetByIdAsync(request.ItemId.Value, ct);

                if (entity is null)
                {
                    entity = await items.AddAsync(new MenuItem
                    {
                        VenueId = request.VenueId,
                        Name = request.Name,
                        Price = request.Price,
                        Description = request.Description,
                        IsAvailable = request.IsAvailable
                    }, ct);
                    logger.LogInformation($"Created new menu item {entity.Id}");
                }
                else
                {
                    entity.VenueId = request.VenueId;
                    entity.Name = request.Name;
                    entity.Price = request.Price;
                    entity.Description = request.Description;
                    entity.IsAvailable = request.IsAvailable;

                    entity = await items.UpdateAsync(entity, ct);
                    logger.LogInformation($"Updated existing menu item {entity.Id}");
                }

                await items.SaveChangesAsync(ct);
                return new MenuItemDto(entity.Id, entity.Name, entity.Price, entity.Description, entity.IsAvailable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(UpsertMenuItem)} for Menu Item: {request.Name}");
                throw;
            }
        }
    }
}