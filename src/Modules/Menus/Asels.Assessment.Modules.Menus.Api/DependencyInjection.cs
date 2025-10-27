using Microsoft.AspNetCore.Routing;

namespace Asels.Assessment.Modules.Menus.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapMenusEndpoints(this IEndpointRouteBuilder app)
    {
        MenuEndpoints.Map(app);
        MenuItemEndpoints.Map(app);
        return app;
    }
}