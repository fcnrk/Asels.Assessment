using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class UpsertMenuItemHandlerTests
{
    [Fact]
    public async Task Create_When_ItemId_Null_Should_Add()
    {
        var venueId = Guid.NewGuid();

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem m, CancellationToken _) => { m.Id = Guid.NewGuid(); return m; });
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpsertMenuItem.Handler(repo.Object, MockLogger.Create<UpsertMenuItem.Handler>());
        var dto = await handler.Handle(new UpsertMenuItem.Command(venueId, null, "Adana Kebap", 325m, "Desc", true), CancellationToken.None);

        dto.Name.Should().Be("Adana Kebap");
        dto.Price.Should().Be(325m);
    }

    [Fact]
    public async Task Update_When_ItemId_Exists_Should_Update()
    {
        var venueId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var existing = new MenuItem { Id = itemId, VenueId = venueId, Name = "Old", Price = 110 };

        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repo.Setup(r => r.UpdateAsync(It.IsAny<MenuItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MenuItem m, CancellationToken _) => m);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpsertMenuItem.Handler(repo.Object, MockLogger.Create<UpsertMenuItem.Handler>());
        var dto = await handler.Handle(new UpsertMenuItem.Command(venueId, itemId, "New", 112.55m, null, true), CancellationToken.None);

        dto.Id.Should().Be(itemId);
        dto.Name.Should().Be("New");
        dto.Price.Should().Be(112.55m);
    }
}