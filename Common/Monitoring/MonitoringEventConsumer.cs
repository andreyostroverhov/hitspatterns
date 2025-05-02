using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using RabbitMQ.Client.Exceptions;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace Common.Monitoring;

public class MonitoringEventConsumer : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MonitoringEventConsumer> _logger;

    public MonitoringEventConsumer(
        IServiceProvider services,
        ILogger<MonitoringEventConsumer> logger)
    {
        _services = services;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = "admin",
            Password = "admin",
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true // Автовосстановление соединения
        };

        const int maxRetries = 15; // Увеличено количество попыток
        var retryCount = 0;
        var connected = false;

        while (retryCount < maxRetries && !connected)
        {
            try
            {
                _logger.LogInformation($"Connection attempt {retryCount + 1}/{maxRetries}");
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(
                    queue: "monitoring_events",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                connected = true;
                _logger.LogInformation("Connected to RabbitMQ successfully");
            }
            catch (Exception ex) when (
                ex is BrokerUnreachableException
                || ex is AuthenticationFailureException
                || ex is SocketException)
            {
                retryCount++;
                _logger.LogWarning($"Connection failed: {ex.Message}");
                Thread.Sleep(10000); // Увеличен интервал до 10 сек
            }
        }

        if (!connected)
            throw new InvalidOperationException($"RabbitMQ connection failed after {maxRetries} attempts");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var monitoringEvent = JsonConvert.DeserializeObject<MonitoringEvent>(message);

                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MonitoringDbContext>();

                    // Добавьте явное указание времени создания события
                    monitoringEvent.Timestamp = DateTime.UtcNow;

                    await dbContext.MonitoringEvents.AddAsync(monitoringEvent);
                    await dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Event saved: {EventId}", monitoringEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing monitoring event");
            }
        };

        _channel.BasicConsume("monitoring_events", true, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}