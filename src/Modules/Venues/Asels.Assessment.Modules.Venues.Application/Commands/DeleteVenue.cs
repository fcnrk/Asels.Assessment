using Asels.Assessment.Modules.Venues.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Venues.Application.Commands;

public static class DeleteVenue
{
    public record Command(Guid Id) : IRequest<bool>;

    public sealed class Handler(IVenueRepository repo, ILogger<Handler> logger) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(DeleteVenue)} for Venue Id: {request.Id}");

            try
            {
                var venue = await repo.GetByIdAsync(request.Id, ct)
                            ?? throw new KeyNotFoundException("Venue not found.");

                var deleted = await repo.DeleteAsync(request.Id, ct);
                if (!deleted) return false;
                await repo.SaveChangesAsync(ct);
                logger.LogInformation($"Deleted venue {venue.Id}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(DeleteVenue)} for VenueVenue Id: {request.Id}");
                throw;
            }
        }
    }
}