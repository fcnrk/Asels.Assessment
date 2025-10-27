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

builder.Services.AddVenuesModule(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var venuesDb = scope.ServiceProvider.GetRequiredService<VenuesDbContext>();
    venuesDb.Database.EnsureCreated(); // for HasData() to work
}

app.MapSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

app.MapVenuesEndpoints();

app.Run();
