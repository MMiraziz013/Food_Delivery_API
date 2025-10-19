namespace Food_Delivery_API.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;
        _logger.LogInformation(" -> Incoming request: {method} {path}", request.Method, request.Path);

        var start = DateTime.UtcNow;
        await next(context);
        var duration = DateTime.UtcNow - start;
        
        _logger.LogInformation("<- Outgoing response: {statusCode} in {duration}ms", context.Response.StatusCode, duration.TotalMilliseconds);
    }
}