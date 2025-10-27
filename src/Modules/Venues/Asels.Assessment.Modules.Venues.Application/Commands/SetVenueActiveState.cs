using Asels.Assessment.Modules.Venues.Application.Interfaces;
using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Venues.Application.Commands;

public static class SetVenueActiveState
{
    public record Command(Guid Id, bool IsActive) : IRequest<VenueDto>;

    public sealed class Handler(IVenueRepository repo, ILogger<Handler> logger) : IRequestHandler<Command, VenueDto>
    {
        public async Task<VenueDto> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(SetVenueActiveState)} for Venue Id: {request.Id}");

            try
            {
                var venue = await repo.GetByIdAsync(request.Id, ct)
                                 ?? throw new KeyNotFoundException("Venue not found.");

                venue.IsActive = request.IsActive;
                await repo.UpdateAsync(venue, ct);
                await repo.SaveChangesAsync(ct);
                logger.LogInformation($"Updated existing venue {venue.Id}");

                return new VenueDto(
                    venue.Id,
                    venue.Name,
                    venue.Description,
                    venue.Address,
                    venue.PhoneNumber,
                    venue.IsActive
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(SetVenueActiveState)} for Venue Id: {request.Id}");
                throw;
            }
        }
    }
}