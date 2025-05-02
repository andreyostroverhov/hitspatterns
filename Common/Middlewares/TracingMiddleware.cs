using Common.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Middlewares;

public class TracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMonitoringPublisher _monitoringPublisher;

    public TracingMiddleware(RequestDelegate next, IMonitoringPublisher monitoringPublisher)
    {
        _next = next;
        _monitoringPublisher = monitoringPublisher;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var monitoringEvent = new MonitoringEvent
            {
                Endpoint = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                DurationMs = stopwatch.ElapsedMilliseconds,
                Timestamp = startTime
            };

            _monitoringPublisher.Publish(monitoringEvent);
        }
    }
}

public static class TracingMiddlewareExtensions
{
    public static IApplicationBuilder UseTracingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TracingMiddleware>();
    }
}
