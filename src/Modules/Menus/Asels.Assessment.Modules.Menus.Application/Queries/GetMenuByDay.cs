using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using MediatR;

namespace Asels.Assessment.Modules.Menus.Application.Queries;

public static class GetMenuByDay
{
    public sealed record Query(Guid VenueId, DayOfWeek Day) : IRequest<MenuDto?>;

    public sealed class Handler(IMenuReadService read) : IRequestHandler<Query, MenuDto?>
    {
        public Task<MenuDto?> Handle(Query request, CancellationToken ct)
            => read.GetActiveByVenueAndDayAsync(request.VenueId, request.Day, ct);
    }
}