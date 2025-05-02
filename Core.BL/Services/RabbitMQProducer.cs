using System;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Enums;
using Core.DAL.Entities;
using System.Text.Json;
using Core.Common.Dtos;

namespace Core.BL.Services
{
    public class RabbitMQProducer
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;

        public RabbitMQProducer(
            string hostName = "rabbitmq",
            string userName = "admin",
            string password = "admin",
            string queueName = "bank_transactions")
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _queueName = queueName;
        }

        // Метод для перевода между счетами
        public void SendTransferMessage(Guid fromAccountId, Guid toAccountId, decimal amount, Currency currency)
        {
            var transactionFrom = new Transaction
            {
                Type = ChangeType.TransferTo,
                AccountId = fromAccountId,
                Amount = amount,
                Currency = currency,
                RelatedAccountId = toAccountId,
                CreatedAt = DateTime.UtcNow
            };

            var transactionTo = new Transaction
            {
                Type = ChangeType.TransferFrom,
                AccountId = toAccountId,
                Amount = amount,
                Currency = currency,
                RelatedAccountId = fromAccountId,
                CreatedAt = DateTime.UtcNow
            };

            SendTransactionMessage(transactionFrom);
            SendTransactionMessage(transactionTo);
        }

        // Метод для операций через "банкомат"
        public void SendMessage(Guid accountId, decimal amount, Currency currency, bool isDeposit)
        {
            var transaction = new Transaction
            {
                Type = isDeposit ? ChangeType.Replenishment : ChangeType.Withdraw,
                AccountId = accountId,
                Amount = amount,
                Currency = currency,
                CreatedAt = DateTime.UtcNow
    };

            SendTransactionMessage(transaction);
        }

        // Основной метод отправки
        private async void SendTransactionMessage(Transaction transaction)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Объявляем очередь
                await channel.QueueDeclareAsync(queue: _queueName,
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                var message = JsonSerializer.Serialize(transaction);
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: "",
                                                routingKey: _queueName,
                                                body: body);
            }
        }
    }
}