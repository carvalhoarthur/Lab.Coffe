using System.Net;
using System.Text.Json;
using Serilog;

namespace Lab.Coffe.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";

        Log.Error(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = exception.Message,
            correlationId = correlationId,
            stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
            innerException = _environment.IsDevelopment() && exception.InnerException != null
                ? new
                {
                    message = exception.InnerException.Message,
                    stackTrace = exception.InnerException.StackTrace
                }
                : null
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
