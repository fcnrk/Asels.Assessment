using Asels.Assessment.Modules.Venues.Contracts.Dtos;
using Asels.Assessment.Modules.Venues.Contracts.ReadApis;
using MediatR;

namespace Asels.Assessment.Modules.Venues.Application.Queries;

public static class GetAllVenues
{
    public record Query : IRequest<IReadOnlyList<VenueDto>>;

    public sealed class Handler(IVenueReadService readService)
        : IRequestHandler<Query, IReadOnlyList<VenueDto>>
    {
        public Task<IReadOnlyList<VenueDto>> Handle(Query request, CancellationToken ct)
            => readService.GetAllAsync(ct);
    }
}