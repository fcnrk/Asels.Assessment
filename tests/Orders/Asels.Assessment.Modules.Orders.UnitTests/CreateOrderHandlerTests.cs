using Asels.Assessment.Modules.Menus.Contracts.Dtos;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using Asels.Assessment.Modules.Orders.Application.Commands;
using Asels.Assessment.Modules.Orders.Application.Interfaces;
using Asels.Assessment.Modules.Orders.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Asels.Assessment.Modules.Orders.UnitTests;

public class CreateOrderHandlerTests
{
    private static CreateOrder.Handler NewHandler(
        out Mock<IMenuReadService> menus,
        out Mock<IOrderRepository> repo)
    {
        menus = new Mock<IMenuReadService>(MockBehavior.Strict);
        repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = Mock.Of<ILogger<CreateOrder.Handler>>();
        return new CreateOrder.Handler(menus.Object, repo.Object, logger);
    }

    private static MenuDto BuildMenu(Guid venueId, DayOfWeek day, params MenuItemDto[] items)
        => new(Guid.NewGuid(), venueId, day, true, items.ToList());

    private static MenuItemDto Item(Guid id, string name, decimal price, bool available = true)
        => new(id, name, price, Description: null, IsAvailable: available);

    [Fact]
    public async Task Creates_order_when_all_items_exist_in_active_menu_and_are_available()
    {
        // Arrange
        var handler = NewHandler(out var menus, out var repo);

        var venueId = Guid.NewGuid();
        var orderDate = new DateOnly(2025, 10, 27); // Monday
        var i1 = Guid.NewGuid();
        var i2 = Guid.NewGuid();

        var menu = BuildMenu(venueId, DayOfWeek.Monday,
            Item(i1, "Soup", 50m),
            Item(i2, "Salad", 40m));

        menus.Setup(m => m.GetActiveByVenueAndDayAsync(venueId, DayOfWeek.Monday, It.IsAny<CancellationToken>()))
             .ReturnsAsync(menu);

        Order? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => { captured = o; return o; });
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var items = new Dictionary<Guid, int>
        {
            [i1] = 2,
            [i2] = 3
        };

        var cmd = new CreateOrder.Command(venueId, orderDate, "test", items);

        // Act
        var dto = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal(2, captured!.Items.Count);
        Assert.Equal(2, captured.Items.First(x => x.MenuItemId == i1).Quantity);
        Assert.Equal(3, captured.Items.First(x => x.MenuItemId == i2).Quantity);
        Assert.Equal(50m * 2 + 40m * 3, captured.Total);
        Assert.Equal(captured.Total, dto.TotalAmount);
        Assert.Equal(orderDate, captured.OrderDate);
        Assert.Equal(venueId, captured.VenueId);

        menus.VerifyAll();
        repo.VerifyAll();
    }

    [Fact]
    public async Task Throws_when_no_active_menu_for_day()
    {
        var handler = NewHandler(out var menus, out var repo);

        var venueId = Guid.NewGuid();
        var orderDate = new DateOnly(2025, 10, 28); // Tuesday
        var i1 = Guid.NewGuid();

        menus.Setup(m => m.GetActiveByVenueAndDayAsync(venueId, DayOfWeek.Tuesday, It.IsAny<CancellationToken>()))
             .ReturnsAsync((MenuDto?)null);

        var items = new Dictionary<Guid, int> { [i1] = 1 };
        var cmd = new CreateOrder.Command(venueId, orderDate, "test", items);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd, CancellationToken.None));

        menus.VerifyAll();
        repo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_when_any_item_not_in_menu_or_unavailable()
    {
        var handler = NewHandler(out var menus, out var repo);

        var venueId = Guid.NewGuid();
        var orderDate = new DateOnly(2025, 10, 27);
        var valid = Guid.NewGuid();
        var unavailable = Guid.NewGuid();
        var notInMenu = Guid.NewGuid();

        var menu = BuildMenu(venueId, DayOfWeek.Monday,
            Item(valid, "Pizza", 100m, available: true),
            Item(unavailable, "Burger", 80m, available: false));

        menus.Setup(m => m.GetActiveByVenueAndDayAsync(venueId, DayOfWeek.Monday, It.IsAny<CancellationToken>()))
             .ReturnsAsync(menu);

        var items = new Dictionary<Guid, int>
        {
            [valid] = 1,
            [unavailable] = 1,
            [notInMenu] = 1
        };

        var cmd = new CreateOrder.Command(venueId, orderDate, "test", items);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd, CancellationToken.None));

        menus.VerifyAll();
        repo.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}