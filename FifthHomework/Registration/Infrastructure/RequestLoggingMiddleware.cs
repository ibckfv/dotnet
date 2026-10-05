using System.Diagnostics;

namespace Registration.Infrastructure;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        using IDisposable? scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = context.TraceIdentifier,
            ["RequestMethod"] = context.Request.Method,
            ["RequestPath"] = context.Request.Path.Value ?? string.Empty
        });

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            logger.LogError(exception, "Unhandled exception while processing HTTP request");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                logger.LogError("HTTP request completed with status {StatusCode} in {ElapsedMilliseconds:F1} ms",
                    statusCode, elapsedMilliseconds);
            }
            else if (statusCode >= StatusCodes.Status400BadRequest)
            {
                logger.LogWarning("HTTP request completed with status {StatusCode} in {ElapsedMilliseconds:F1} ms",
                    statusCode, elapsedMilliseconds);
            }
            else
            {
                logger.LogInformation("HTTP request completed with status {StatusCode} in {ElapsedMilliseconds:F1} ms",
                    statusCode, elapsedMilliseconds);
            }
        }
    }
}
