using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class PatchMenuItemHandlerTests
{
    [Fact]
    public async Task Patch_Should_Update_Only_Provided_Fields()
    {
        var venueId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = venueId, Name = "Old", Price = 5, IsAvailable = false };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repo.Setup(r => r.UpdateAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem m, CancellationToken _) => m);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new PatchMenuItem.Handler(repo.Object, MockLogger.Create<PatchMenuItem.Handler>());
        var dto = await handler.Handle(new PatchMenuItem.Command(venueId, itemId, null, 12m, null, true), CancellationToken.None);

        dto!.Price.Should().Be(12m);
        dto.IsAvailable.Should().BeTrue();
        dto.Name.Should().Be("Old"); // unchanged
    }

    [Fact]
    public async Task Patch_Returns_Null_When_Item_Not_Owned_By_Venue()
    {
        var venueId = Guid.NewGuid();
        var otherVenue = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = otherVenue, Name = "X", Price = 10 };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var handler = new PatchMenuItem.Handler(repo.Object, MockLogger.Create<PatchMenuItem.Handler>());
        var act = () => handler.Handle(new PatchMenuItem.Command(venueId, itemId, "N", null, null, null), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}