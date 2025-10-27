using Asels.Assessment.Modules.Venues.Application.Commands;
using Asels.Assessment.Modules.Venues.Application.Interfaces;
using Asels.Assessment.Modules.Venues.Domain.Entities;
using Moq;
using FluentAssertions;

namespace Asels.Assessment.Modules.Venues.UnitTests;

public class UpsertVenueHandlerTests
{
    [Fact]
    public async Task Create_And_Return_Dto()
    {
        var repo = new Mock<IVenueRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Venue?)null);
        repo.Setup(r => r.AddAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Venue v, CancellationToken _) => { v.Id = Guid.NewGuid(); return v; });
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpsertVenue.Handler(repo.Object, MockLogger.Create<UpsertVenue.Handler>());
        var cmd = new UpsertVenue.Command(
            Id: null,
            Name: "Aralık Sonu Ocakbaşı",
            Description: "Kebap",
            Address: "Tunalı Hilmi Cd. No 45",
            PhoneNumber: "+905551111111",
            IsActive: true
        );

        var dto = await handler.Handle(cmd, CancellationToken.None);

        dto.Should().NotBeNull();
        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("Aralık Sonu Ocakbaşı");
        repo.Verify(r => r.AddAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_And_Return_Dto()
    {
        var existing = new Venue
        {
            Id = Guid.NewGuid(),
            Name = "Old name",
            Description = "Old desc",
            Address = "Old addr",
            PhoneNumber = "000",
            IsActive = true
        };

        var repo = new Mock<IVenueRepository>();
        repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        repo.Setup(r => r.UpdateAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Venue v, CancellationToken _) => v);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpsertVenue.Handler(repo.Object, MockLogger.Create<UpsertVenue.Handler>());
        var cmd = new UpsertVenue.Command(
            Id: existing.Id,
            Name: "New Name",
            Description: "New desc",
            Address: "New addr",
            PhoneNumber: "111",
            IsActive: false
        );

        // Act
        var dto = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        dto.Id.Should().Be(existing.Id);
        dto.Name.Should().Be("New Name");
        dto.Description.Should().Be("New desc");
        dto.Address.Should().Be("New addr");
        dto.PhoneNumber.Should().Be("111");
        dto.IsActive.Should().BeFalse();
        repo.Verify(r => r.UpdateAsync(It.Is<Venue>(v => v.Id == existing.Id), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()), Times.Never);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}