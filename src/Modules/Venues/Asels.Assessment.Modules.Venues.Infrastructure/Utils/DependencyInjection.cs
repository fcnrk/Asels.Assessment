using Asels.Assessment.Modules.Venues.Application.Interfaces;
using Asels.Assessment.Modules.Venues.Application.Utils;
using Asels.Assessment.Modules.Venues.Contracts.ReadApis;
using Asels.Assessment.Modules.Venues.Infrastructure.Persistence;
using Asels.Assessment.Modules.Venues.Infrastructure.Repositories;
using Asels.Assessment.Modules.Venues.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Asels.Assessment.Modules.Venues.Infrastructure.Utils;

public static class DependencyInjection
{
    public static IServiceCollection AddVenuesModule(this IServiceCollection services, IConfiguration config, bool ensureIndexes = true)
    {
        services.AddDbContext<VenuesDbContext>(opt => opt.UseInMemoryDatabase("venues-db"));
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IVenueReadService, VenueReadService>();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.Utils.AssemblyMarker).Assembly);
        });
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(VenuesLoggingBehavior<,>));

        return services;
    }
}