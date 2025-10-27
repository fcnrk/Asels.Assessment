using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using MediatR;

namespace Asels.Assessment.Modules.Menus.Application.Queries;

public static class ListMenusByVenue
{
    public sealed record Query(Guid VenueId) : IRequest<IReadOnlyList<MenuDto>>;

    public sealed class Handler(IMenuReadService read) : IRequestHandler<Query, IReadOnlyList<MenuDto>>
    {
        public Task<IReadOnlyList<MenuDto>> Handle(Query request, CancellationToken ct) =>
            read.GetAllByVenueAsync(request.VenueId, ct);

    }
}