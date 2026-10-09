using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling Request: {RequestName}", requestName);

        var timer = Stopwatch.StartNew();

        var response = await next();

        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds} ms)",
                requestName, timer.ElapsedMilliseconds);
        }

        _logger.LogInformation("Handled Request: {RequestName}", requestName);

        return response;
    }
}