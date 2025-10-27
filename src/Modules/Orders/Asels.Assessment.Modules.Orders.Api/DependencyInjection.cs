using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Orders.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        Endpoints.Map(app);
        return app;
    }
}