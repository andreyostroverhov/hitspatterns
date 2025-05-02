using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Monitoring;

public interface IMonitoringPublisher
{
    void Publish(MonitoringEvent monitoringEvent);
}
