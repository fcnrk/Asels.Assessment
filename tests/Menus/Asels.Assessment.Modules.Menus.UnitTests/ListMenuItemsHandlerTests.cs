using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Application.Queries;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class ListMenuItemsHandlerTests
{
    [Fact]
    public async Task List_Returns_Sorted_By_Name()
    {
        var venueId = Guid.NewGuid();
        var repo = new Mock<IMenuItemRepository>();
        repo.Setup(r => r.GetAllByVenueAsync(venueId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MenuItem>
            {
                new() { Id = Guid.NewGuid(), VenueId = venueId, Name = "B", Price = 2 },
                new() { Id = Guid.NewGuid(), VenueId = venueId, Name = "A", Price = 1 },
            });

        var handler = new ListMenuItems.Handler(repo.Object, MockLogger.Create<ListMenuItems.Handler>());
        var list = await handler.Handle(new ListMenuItems.Query(venueId), CancellationToken.None);

        list.Should().HaveCount(2);
        list[0].Name.Should().Be("A");
        list[1].Name.Should().Be("B");
    }
}