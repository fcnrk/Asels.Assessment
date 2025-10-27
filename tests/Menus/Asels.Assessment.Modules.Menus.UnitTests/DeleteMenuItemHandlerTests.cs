using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class DeleteMenuItemHandlerTests
{
    [Fact]
    public async Task Delete_Returns_True_When_Found()
    {
        var venueId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = venueId, Name = "X", Price = 10, IsAvailable = true };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repo.Setup(r => r.DeleteAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteMenuItem.Handler(repo.Object, MockLogger.Create<DeleteMenuItem.Handler>());
        var ok = await handler.Handle(new DeleteMenuItem.Command(venueId, itemId), CancellationToken.None);

        ok.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Returns_False_When_NotFound()
    {
        var venueId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync((MenuItem?)null);

        var handler = new DeleteMenuItem.Handler(repo.Object, MockLogger.Create<DeleteMenuItem.Handler>());
        var act = () => handler.Handle(new DeleteMenuItem.Command(venueId, itemId), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}