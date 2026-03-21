using System.Net;
using System.Text.Json;

namespace Library.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API Error: {ex}");

            await HandleException(context, HttpStatusCode.InternalServerError, "Internal server error");
        }
    }

    private static async Task HandleException(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new
        {
            status = context.Response.StatusCode,
            error = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}