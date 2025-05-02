using System;
using RabbitMQ.Client;
using Core.Common.Enums;
using Core.DAL.Entities;
using System.Text;
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

        private void SendTransactionMessage(Transaction transaction)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = JsonSerializer.Serialize(transaction);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                body: body);
        }
    }
}