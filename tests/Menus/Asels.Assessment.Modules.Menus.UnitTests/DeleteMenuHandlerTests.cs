using Asels.Assessment.Modules.Menus.Application.Commands;
using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Domain.Entities;
using Asels.Assessment.Tests.Common;
using FluentAssertions;
using Moq;

namespace Asels.Assessment.Modules.Menus.UnitTests;

public class DeleteMenuHandlerTests
{
    [Fact]
    public async Task Delete_Returns_True_When_Deleted()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IMenuRepository>();
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(new Menu(id: id));
        repo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteMenu.Handler(repo.Object, MockLogger.Create<DeleteMenu.Handler>());

        var result = await handler.Handle(new DeleteMenu.Command(id), CancellationToken.None);

        result.Should().BeTrue();
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Throws_When_NotFound()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IMenuRepository>();

        repo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteMenu.Handler(repo.Object, MockLogger.Create<DeleteMenu.Handler>());

        var act = () => handler.Handle(new DeleteMenu.Command(id), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>(); 
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}