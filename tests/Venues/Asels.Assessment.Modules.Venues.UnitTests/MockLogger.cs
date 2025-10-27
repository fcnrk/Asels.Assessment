using Microsoft.Extensions.Logging;
using Moq;

namespace Asels.Assessment.Modules.Venues.UnitTests;

public static class MockLogger
{
    public static ILogger<T> Create<T>() => new Mock<ILogger<T>>().Object;
}