using Microsoft.AspNetCore.Mvc;

namespace MAPCsDashboard.Api.Middleware;

public sealed class GetOnlyMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> AllowedMethods =
        new(StringComparer.OrdinalIgnoreCase) { "GET", "HEAD", "OPTIONS" };

    public async Task InvokeAsync(HttpContext context)
    {
        if (!AllowedMethods.Contains(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            context.Response.Headers.Allow = "GET, HEAD, OPTIONS";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status405MethodNotAllowed,
                Title = "Operación no permitida",
                Detail = "Esta API es de solo lectura y acepta únicamente consultas GET."
            });
            return;
        }

        await next(context);
    }
}

