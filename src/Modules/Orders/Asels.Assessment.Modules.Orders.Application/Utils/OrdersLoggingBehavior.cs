using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Asels.Assessment.Modules.Orders.Application.Utils;

public sealed class OrdersLoggingBehavior<TRequest, TResponse>(
    ILogger<OrdersLoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestNamespace = typeof(TRequest).Namespace ?? string.Empty;
        if (!requestNamespace.Contains(".Orders.", StringComparison.OrdinalIgnoreCase))
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation($"[Orders] Handling {requestName}: {@request}");

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();

            logger.LogInformation($"[Orders] Handled {requestName} in {stopwatch.ElapsedMilliseconds}ms");
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Orders] Error handling {requestName}: {@request}");
            throw;
        }
    }
}