using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class UpsertMenuHandlerTests
{
    [Fact]
    public async Task UpsertMenu_And_Return_Details()
    {
        var venueId = Guid.NewGuid();
        var day = DayOfWeek.Monday;
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var menuRepo = new Mock<IMenuRepository>();
        var itemRepo = new Mock<IMenuItemRepository>();

        menuRepo.Setup(r => r.GetActiveByVenueAndDayAsync(venueId, day, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Menu?)null);
        menuRepo.Setup(r => r.AddAsync(It.IsAny<Menu>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Menu m, CancellationToken _) => { m.Id = Guid.NewGuid(); return m; });
        menuRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        itemRepo.Setup(r => r.GetByIdsAsync(venueId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid v, IEnumerable<Guid> reqIds, CancellationToken _) =>
                reqIds.Select(id => new MenuItem { Id = id, VenueId = v, Name = $"Item-{id}", Price = 10 }).ToList());

        var handler = new UpsertMenu.Handler(menuRepo.Object, itemRepo.Object, MockLogger.Create<UpsertMenu.Handler>());

        var dto = await handler.Handle(new UpsertMenu.Command(venueId, day, ids, true), CancellationToken.None);

        dto.Should().NotBeNull();
        dto.VenueId.Should().Be(venueId);
        dto.Day.Should().Be(day);
        dto.IsActive.Should().BeTrue();
        dto.Items.Select(i => i.Id).Should().BeEquivalentTo(ids, options => options.WithoutStrictOrdering());
        dto.Items.All(i => i.Name.StartsWith("Item-")).Should().BeTrue();

        menuRepo.Verify(r => r.DeactivateOtherMenusAsync(venueId, day, dto.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertMenu_Should_Throw_When_AnyItemId_NotBelongToVenue()
    {
        var venueId = Guid.NewGuid();
        var day = DayOfWeek.Tuesday;
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var menuRepo = new Mock<IMenuRepository>();
        var itemRepo = new Mock<IMenuItemRepository>();

        // only return 2 of 3 items to simulate missing one
        itemRepo.Setup(r => r.GetByIdsAsync(venueId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid v, IEnumerable<Guid> reqIds, CancellationToken _) =>
                reqIds.Take(2).Select(id => new MenuItem { Id = id, VenueId = v, Name = $"Item-{id}", Price = 10 }).ToList());

        var handler = new UpsertMenu.Handler(menuRepo.Object, itemRepo.Object, MockLogger.Create<UpsertMenu.Handler>());

        var act = () => handler.Handle(new UpsertMenu.Command(venueId, day, ids, true), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        menuRepo.Verify(r => r.AddAsync(It.IsAny<Menu>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}