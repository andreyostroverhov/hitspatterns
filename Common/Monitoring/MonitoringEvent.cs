using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Monitoring;

public class MonitoringEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ServiceName { get; set; }
    public string Endpoint { get; set; }
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsError => StatusCode >= 500;
}
