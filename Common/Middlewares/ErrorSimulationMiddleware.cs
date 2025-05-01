using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Middlewares;

public class ErrorSimulationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Random _random = new();

    public ErrorSimulationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool isEvenMinute = DateTime.Now.Minute % 2 == 0;

        double errorProbability = isEvenMinute ? 0.9 : 0.5;

        if (_random.NextDouble() < errorProbability)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error (simulated)");
            return;
        }

        await _next(context);
    }
}

public static class ErrorSimulationMiddlewareExtension
{
    public static IApplicationBuilder UseErrorSimulationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorSimulationMiddleware>();
    }
}
