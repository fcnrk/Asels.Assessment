using Asels.Assessment.Modules.Venues.Application.Interfaces;
using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using Asels.Assessment.Modules.Venues.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Venues.Application.Commands;

public static class UpsertVenue
{
    public record Command(
        Guid? Id,
        string Name,
        string? Description,
        string? Address,
        string? PhoneNumber,
        bool IsActive = true
    ) : IRequest<VenueDto>;

    public sealed class Handler(IVenueRepository repo, ILogger<Handler> logger)
        : IRequestHandler<Command, VenueDto>
    {
        public async Task<VenueDto> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation($"Handling {nameof(UpsertVenue)} for Venue: {request.Name}");
            Venue? venue = null;

            try
            {
                if (request.Id.HasValue)
                    venue = await repo.GetByIdAsync(request.Id.Value, ct);

                if (venue is null)
                {
                    venue = new Venue
                    {
                        Id = request.Id ?? Guid.NewGuid(),
                        Name = request.Name,
                        Description = request.Description,
                        Address = request.Address,
                        PhoneNumber = request.PhoneNumber,
                        IsActive = request.IsActive
                    };

                    await repo.AddAsync(venue, ct);
                    logger.LogInformation($"Created new venue {venue.Id}");
                }
                else
                {
                    venue.Name = request.Name;
                    venue.Description = request.Description;
                    venue.Address = request.Address;
                    venue.PhoneNumber = request.PhoneNumber;
                    venue.IsActive = request.IsActive;

                    await repo.UpdateAsync(venue, ct);
                    logger.LogInformation($"Updated existing venue {venue.Id}");
                }

                await repo.SaveChangesAsync(ct);

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
                logger.LogError(ex, $"Error handling {nameof(UpsertVenue)} for Venue: {request.Name}");
                throw;
            }
        }
    }
}