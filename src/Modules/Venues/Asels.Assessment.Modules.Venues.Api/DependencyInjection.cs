using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Venues.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapVenuesEndpoints(this IEndpointRouteBuilder app)
    {
        Endpoints.Map(app);
        return app;
    }
}