using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using Asels.Assessment.Modules.Venues.Contracts.ReadApis;
using MediatR;

namespace Asels.Assessment.Modules.Venues.Application.Queries;

public static class GetVenueById
{
    public record Query(Guid Id) : IRequest<VenueDto?>;

    public sealed class Handler(IVenueReadService readService)
        : IRequestHandler<Query, VenueDto?>
    {
        public Task<VenueDto?> Handle(Query request, CancellationToken ct)
            => readService.GetByIdAsync(request.Id, ct);
    }
}