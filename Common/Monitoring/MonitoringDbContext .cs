using Microsoft.EntityFrameworkCore;

namespace Common.Monitoring;

public class MonitoringDbContext : DbContext
{
    public DbSet<MonitoringEvent> MonitoringEvents { get; set; }

    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options) { }

}