using Asels.Assessment.Modules.Menus.Application.Interfaces;
using Asels.Assessment.Modules.Menus.Application.Utils;
using Asels.Assessment.Modules.Menus.Contracts.ReadApis;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Asels.Assessment.Modules.Menus.Infrastructure.Repositories;
using Asels.Assessment.Modules.Menus.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asels.Assessment.Modules.Menus.Infrastructure.Utils;

public static class DependencyInjection
{
    public static IServiceCollection AddMenusModule(this IServiceCollection services, IConfiguration config, bool ensureIndexes = true)
    {
        services.AddDbContext<MenusDbContext>(opt => opt.UseInMemoryDatabase("menus-db"));
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IMenuReadService, MenuReadService>();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.Utils.AssemblyMarker).Assembly);
        });
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MenusLoggingBehavior<,>));

        return services;
    }
}