namespace Web.Api.Middleware;

public class CancellationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CancellationMiddleware> _logger;

    public CancellationMiddleware(RequestDelegate next, ILogger<CancellationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Request was canceled by the client. Path: {Path}", context.Request.Path);

            if (!context.Response.HasStarted) // if its not too late to modify the response
            {
                context.Response.StatusCode = 499; // Client Closed Request
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new { message = "The request was canceled by the client." });
            }
        }
    }

}
