using Core.Common.Dtos;
using Core.Common.Interfaces;
using Core.DAL;
using Core.DAL.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Core.BL.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(
            IServiceProvider serviceProvider,
            string hostName = "rabbitmq",
            string userName = "admin",
            string password = "admin",
            string queueName = "bank_transactions")
        {
            _serviceProvider = serviceProvider;
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _queueName = queueName;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitializeRabbitMQ();
            StartConsuming();
            return Task.CompletedTask;
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                DispatchConsumersAsync = true // Для асинхронных потребителей
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(0, 1, false);
        }

        private void StartConsuming()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var transaction = JsonSerializer.Deserialize<Transaction>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                    await transactionService.SaveTransactionToDatabase(transaction);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}