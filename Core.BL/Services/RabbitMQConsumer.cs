using Core.Common.Dtos;
using Core.Common.Interfaces;
using Core.DAL;
using Core.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Core.BL.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _hostName;
        private readonly string _queueName;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQConsumer(IServiceProvider serviceProvider, string hostName = "localhost", string queueName = "bank_transactions")
        {
            _serviceProvider = serviceProvider;
            _hostName = hostName;
            _queueName = queueName;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                InitializeRabbitMQ();
                StartConsuming(stoppingToken);
            }, stoppingToken);
        }

        private async void InitializeRabbitMQ() 
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            {
               await _channel.QueueDeclareAsync(queue: _queueName,
                                               durable: true,
                                               exclusive: false,
                                               autoDelete: false,
                                               arguments: null);
              
               _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            }
        }

        private void StartConsuming(CancellationToken stoppingToken) 
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var transaction = JsonSerializer.Deserialize<Transaction>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    // Получаем Scoped-сервис из Scope
                    var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

                    // Используем сервис
                    await transactionService.SaveTransactionToDatabase(transaction);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsumeAsync(queue: _queueName,
                                       autoAck: false,
                                       consumer: consumer);

            // Ожидаем сигнала остановки
            while (!stoppingToken.IsCancellationRequested)
            {
                Task.Delay(1000, stoppingToken).Wait(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
