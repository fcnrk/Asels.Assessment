using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Menus.Application.Commands;

public static class UpsertMenu
{
    public sealed record Command(
        Guid VenueId,
        DayOfWeek Day,
        IReadOnlyList<Guid> ItemIds, // only IDs
        bool Activate = true
    ) : IRequest<MenuDto>;

    public sealed class Handler(
        IMenuRepository menus,
        IMenuItemRepository items,
        ILogger<Handler> logger
    ) : IRequestHandler<Command, MenuDto>
    {
        public async Task<MenuDto> Handle(Command request, CancellationToken ct)
        {
            logger.LogInformation(
                $"[Menus] UpsertMenu (Venue={request.VenueId}, Day={request.Day}, Activate={request.Activate})");

            try
            {
                var uniqueIds = request.ItemIds.Distinct().ToList();
                var found = uniqueIds.Count == 0
                    ? new List<MenuItem>()
                    : await items.GetByIdsAsync(request.VenueId, uniqueIds, ct);

                if (found.Count != uniqueIds.Count)
                {
                    var foundSet = found.Select(i => i.Id).ToHashSet();
                    var missing = uniqueIds.Where(id => !foundSet.Contains(id)).ToList();
                    throw new KeyNotFoundException(
                        $"One or more menu items were not found for this venue: {string.Join(", ", missing)}");
                }

                var existing = await menus.GetActiveByVenueAndDayAsync(request.VenueId, request.Day, ct);
                Menu menu;
                if (existing is null)
                {
                    menu = new Menu
                    {
                        VenueId = request.VenueId,
                        Day = request.Day,
                        IsActive = request.Activate
                    };
                    menu = await menus.AddAsync(menu, ct);
                }
                else
                {
                    menu = existing;
                    menu.IsActive = request.Activate;
                    menu.UpdatedUtc = DateTime.UtcNow;
                    menu.Entries.Clear();
                    menu = await menus.UpdateAsync(menu, ct);
                }

                foreach (var id in uniqueIds)
                {
                    menu.Entries.Add(new MenuEntry { MenuItemId = id });
                }

                if (request.Activate)
                    await menus.DeactivateOtherMenusAsync(request.VenueId, request.Day, menu.Id, ct);

                await menus.SaveChangesAsync(ct);

                var byId = found.ToDictionary(i => i.Id);
                var dtoItems = uniqueIds
                    .Select(id =>
                    {
                        var mi = byId[id];
                        return new MenuItemDto(mi.Id, mi.Name, mi.Price, mi.Description, mi.IsAvailable);
                    })
                    .ToList();

                var dto = new MenuDto(menu.Id, menu.VenueId, menu.Day, menu.IsActive, dtoItems);
                logger.LogInformation($"[Menus] {nameof(UpsertMenu)} completed for MenuId={menu.Id}");
                return dto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling {nameof(UpsertMenu)} for (Venue={request.VenueId}, Day={request.Day}, Activate={request.Activate}).");
                throw;
            }
        }
    }
}