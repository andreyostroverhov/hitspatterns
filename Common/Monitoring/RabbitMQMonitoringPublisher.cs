using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Common.Monitoring;

public class RabbitMQMonitoringPublisher : IMonitoringPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _serviceName;
    private readonly ILogger<RabbitMQMonitoringPublisher> _logger;

    public RabbitMQMonitoringPublisher(
        string hostName,
        string userName,
        string password,
        string serviceName,
        ILogger<RabbitMQMonitoringPublisher> logger)
    {
        _serviceName = serviceName;
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection(); // IConnection
            _channel = _connection.CreateModel(); // IModel
            _channel.QueueDeclare(
                queue: "monitoring_events",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    public void Publish(MonitoringEvent monitoringEvent)
    {
        try
        {
            monitoringEvent.ServiceName = _serviceName;
            var message = JsonConvert.SerializeObject(monitoringEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "monitoring_events",
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish monitoring event");
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Dispose(); // Вместо Close()
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during disposal");
        }
    }
}