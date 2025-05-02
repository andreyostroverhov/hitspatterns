using Common.Monitoring;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/monitoring")]
public class MonitoringController : ControllerBase
{
    private readonly MonitoringDbContext _context;

    public MonitoringController(MonitoringDbContext context)
    {
        _context = context;
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        var events = _context.MonitoringEvents.ToList();

        var hourlyStats = events
            .GroupBy(e => e.Timestamp.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                TotalRequests = g.Count(),
                ErrorCount = g.Count(e => e.IsError),
                ErrorRate = g.Any() ? Math.Round((double)g.Count(e => e.IsError) / g.Count() * 100, 2) : 0,
                AvgDuration = g.Average(e => e.DurationMs),
                StatusCodes = g.GroupBy(e => e.StatusCode)
                                .ToDictionary(sg => sg.Key, sg => sg.Count())
            })
            .OrderBy(x => x.Hour)
            .ToList();

        return Ok(new
        {
            Summary = new
            {
                TotalRequests = events.Count,
                TotalErrors = events.Count(e => e.IsError),
                GlobalErrorRate = events.Any()
                    ? Math.Round((double)events.Count(e => e.IsError) / events.Count * 100, 2)
                    : 0
            },
            HourlyStats = hourlyStats.Select(h => new {
                Time = $"{h.Hour:00}:00-{h.Hour + 1:00}:00",
                h.TotalRequests,
                h.ErrorCount,
                h.ErrorRate,
                AvgDurationMs = Math.Round(h.AvgDuration, 2),
                h.StatusCodes
            })
        });
    }
}