using Asels.Assessment.Modules.Menus.Api;
using Asels.Assessment.Modules.Menus.Infrastructure.Persistence;
using Asels.Assessment.Modules.Menus.Infrastructure.Utils;
using Asels.Assessment.Modules.Venues.Api;
using Asels.Assessment.Modules.Venues.Infrastructure.Persistence;
using Asels.Assessment.Modules.Venues.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddMenusModule(builder.Configuration);
builder.Services.AddVenuesModule(builder.Configuration);

var app = builder.Build();

// for HasData() to work
using (var scope = app.Services.CreateScope())
{
    var menusDb = scope.ServiceProvider.GetRequiredService<MenusDbContext>();
    var venuesDb = scope.ServiceProvider.GetRequiredService<VenuesDbContext>();
    menusDb.Database.EnsureCreated();
    venuesDb.Database.EnsureCreated(); 
}

app.MapSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

app.MapVenuesEndpoints();
app.MapMenusEndpoints();

app.Run();
