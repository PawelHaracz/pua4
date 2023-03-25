using Microsoft.Extensions.Primitives;

namespace PUA4.Observabilities;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderKey = Constants.CorrelationIdHeaderKey;
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    public CorrelationIdMiddleware(RequestDelegate next,
        ILoggerFactory loggerFactory)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = loggerFactory.CreateLogger<CorrelationIdMiddleware>();
    }
    public async Task Invoke(HttpContext httpContext)
    {
        string correlationId = string.Empty;
        if (httpContext.Request.Headers.TryGetValue(
                CorrelationIdHeaderKey, out StringValues correlationIds))
        {
            correlationId = correlationIds.FirstOrDefault(k =>  k.Equals(CorrelationIdHeaderKey)) ?? throw new Exception($"missing correlationId value in {CorrelationIdHeaderKey}");
            _logger.LogInformation("CorrelationId from Request Header:{correlationId}", correlationId);
        }
        else
        {
            correlationId = Guid.NewGuid().ToString("N");
            httpContext.Request.Headers.Add(CorrelationIdHeaderKey,correlationId);
            _logger.LogInformation("Generated CorrelationId: {correlationId}", correlationId);
        }
        httpContext.Response.OnStarting(() =>
        {
            if (!httpContext.Response.Headers.
                    TryGetValue(CorrelationIdHeaderKey,
                        out correlationIds))
                httpContext.Response.Headers.Add(
                    CorrelationIdHeaderKey, correlationId);
            return Task.CompletedTask;
        });
        await _next.Invoke(httpContext);
    }
}