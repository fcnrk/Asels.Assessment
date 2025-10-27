using Asels.Assessment.Modules.Menus.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Commands;

public static class DeleteMenuItem
{
    public sealed record Command(Guid VenueId, Guid ItemId) : IRequest<bool>;

    public sealed class Handler(
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(DeleteMenuItem)} for Item Id: {request.ItemId}");

            try
            {
                var entity = await items.GetByIdAsync(request.ItemId, ct);
                if (entity is null || entity.VenueId != request.VenueId)
                    throw new KeyNotFoundException($"Item {request.ItemId} not found for Venue {request.VenueId}.");

                var deleted = await items.DeleteAsync(request.ItemId, ct);
                if (!deleted) return false;
                await items.SaveChangesAsync(ct);
                logger.LogInformation($"Deleted Menu Item with Id: {entity.Id}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(DeleteMenuItem)} for Menu Item Id: {request.ItemId}");
                throw;
            }
        }
    }
}