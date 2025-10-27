using Asels.Assessment.Modules.Orders.Application.Interfaces;
using Asels.Assessment.Modules.Orders.Application.Utils;
using Asels.Assessment.Modules.Orders.Infrastructure.Persistence;
using Asels.Assessment.Modules.Orders.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asels.Assessment.Modules.Orders.Infrastructure.Utils;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration config, bool ensureIndexes = true)
    {
        services.AddDbContext<OrdersDbContext>(opt => opt.UseInMemoryDatabase("orders-db"));
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.Utils.AssemblyMarker).Assembly);
        });
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OrdersLoggingBehavior<,>));

        return services;
    }
    
}