using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Venues.Application.Utils;

public sealed class VenuesLoggingBehavior<TRequest, TResponse>(
    ILogger<VenuesLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestNamespace = typeof(TRequest).Namespace ?? string.Empty;
        if (!requestNamespace.Contains(".Venues.", StringComparison.OrdinalIgnoreCase))
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation($"[Venues] Handling {requestName}: {@request}");

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();

            logger.LogInformation($"[Venues] Handled {requestName} in {stopwatch.ElapsedMilliseconds}ms");
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Venues] Error handling {requestName}: {@request}");
            throw;
        }
    }
}