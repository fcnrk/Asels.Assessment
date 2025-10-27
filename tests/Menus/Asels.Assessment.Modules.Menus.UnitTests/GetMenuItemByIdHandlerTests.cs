using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Application.Queries;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class GetMenuItemByIdHandlerTests
{
    [Fact]
    public async Task Get_Returns_Dto_When_Owned()
    {
        var venueId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = venueId, Name = "X", Price = 10 };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var handler = new GetMenuItemById.Handler(repo.Object, MockLogger.Create<GetMenuItemById.Handler>());
        var dto = await handler.Handle(new GetMenuItemById.Query(venueId, itemId), CancellationToken.None);

        dto.Should().NotBeNull();
        dto!.Id.Should().Be(itemId);
    }

    [Fact]
    public async Task Get_Returns_Null_When_NotOwned()
    {
        var venueId = Guid.NewGuid();
        var other = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = other, Name = "X", Price = 10 };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var handler = new GetMenuItemById.Handler(repo.Object, MockLogger.Create<GetMenuItemById.Handler>());
        var dto = await handler.Handle(new GetMenuItemById.Query(venueId, itemId), CancellationToken.None);

        dto.Should().BeNull();
    }
}