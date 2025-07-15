using System.Net;
using System.Text.Json;
using Application.Abstractions.Authentication;
using SharedKernel.Exceptions;

namespace Instagram_Backend.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var userContext = context.RequestServices.GetRequiredService<IUserContext>();

        var userId = userContext.UserId.ToString() ?? "anonymous";
        _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}, Query: {Query}, Message: {msg}",
            context.Request.Path,
            context.Request.Method,
            userId,
            context.Request.QueryString,
            ex.Message);
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred";

        if (ex is BaseException baseException)
        {
            statusCode = (HttpStatusCode)baseException.StatusCode;
            message = baseException.Message;
        }
        else if (ex is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "Unauthorized";
        }

        context.Response.StatusCode = (int)statusCode;


        var response = new
        {
            message,
            details = _env.IsDevelopment() ? ex.ToString() : null,
            data = false
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}

