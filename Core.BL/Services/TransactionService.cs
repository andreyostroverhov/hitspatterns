using Common.Exceptions;
using Core.BL.Hubs;
using Core.Common.Dtos;
using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.DAL;
using Core.DAL.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly CoreDbContext _CoreDbContext;
        private readonly Converter _Converter;
        private readonly RabbitMQProducer _Producer;
        private readonly IHubContext<HistoryHub> _hubContext;
        public TransactionService(CoreDbContext coreDbContext, Converter converter, RabbitMQProducer producer, IHubContext<HistoryHub> hubContext)
        {
            _CoreDbContext = coreDbContext;
            _Converter = converter;
            _Producer = producer;
            _hubContext = hubContext;
        }
        public async Task<BADto> ReplenishmentMoney(Guid Id, decimal Amount, Currency currency)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == Id) ?? throw new NotFoundException("Bank account not found.");

            var newAmount = await _Converter.Convert(currency, bankAcc.Currency, Amount);

            bankAcc.Amount = bankAcc.Amount + newAmount;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            _Producer.SendMessage(bankAcc.Id, Amount, currency, true);
            //AddRecord(Common.Enums.ChangeType.Replenishment, bankAcc.Id); 


            return new BADto
            {
                Id = bankAcc.Id,
                Currency = bankAcc.Currency,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<BADto> WithdrawMoney(Guid baId, Guid clientId, decimal Amount, Currency currency)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            if (bankAcc.ClientId != clientId)
            {
                throw new ServiceUnavailableException("You do not have access to this bank account.");
            }

            var newAmount = await _Converter.Convert(currency, bankAcc.Currency, Amount);

            if (bankAcc.Amount < newAmount)
            {
                throw new ForbiddenException("Not enough funds");
            }

            bankAcc.Amount = bankAcc.Amount - newAmount;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            _Producer.SendMessage(bankAcc.Id, Amount, currency, false);
            //AddRecord(Common.Enums.ChangeType.Withdraw, bankAcc.Id);

            return new BADto
            {
                Id = bankAcc.Id,
                Currency = bankAcc.Currency,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<BADto> TransferMoney(Guid clientId, Guid from, Guid to, decimal amount, Currency currency)
        {
            var bankAccFrom = await _CoreDbContext.BankAccounts
               .FirstOrDefaultAsync(ba => ba.Id == from) ?? throw new NotFoundException("Bank account not found.");

            if (bankAccFrom.ClientId != clientId)
            {
                throw new ServiceUnavailableException("You do not have access to this bank account.");
            }

            var bankAccTo = await _CoreDbContext.BankAccounts
               .FirstOrDefaultAsync(ba => ba.Id == to) ?? throw new NotFoundException("Bank account not found.");

            var minusAmount = await _Converter.Convert(bankAccFrom.Currency, currency, amount);
            var plusAmount = await _Converter.Convert(bankAccTo.Currency, currency, amount);

            if (bankAccFrom.Amount < minusAmount)
            {
                throw new ForbiddenException("Not enough funds");
            }

            bankAccFrom.Amount = bankAccFrom.Amount - minusAmount;
            bankAccTo.Amount = bankAccTo.Amount + plusAmount;

            _CoreDbContext.BankAccounts.UpdateRange(bankAccFrom, bankAccTo);
            await _CoreDbContext.SaveChangesAsync();

            _Producer.SendTransferMessage(from, to, amount, currency);

            return new BADto
            {
                Id = bankAccFrom.Id,
                Amount = bankAccFrom.Amount,
                CreatedAt = bankAccFrom.CreatedAt,
                Status = bankAccFrom.Status
            };
        }

        public async Task<List<StoryDto>> GetTransactions(Guid baId)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
               .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            var story = await _CoreDbContext.ChangeEvents
               .AsNoTracking()
               .Where(ce => ce.BankAccId == baId)
               .Select(ce => new StoryDto
               {
                   Id = ce.Id,
                   BankAccId = ce.BankAccId,
                   CreatedAt = ce.CreatedAt,
                   Type = ce.Type,
                   RelatedAccountId = ce.RelatedAccountId,
                   Amount = ce.Amount,
                   Currency = ce.Currency
               })
               .ToListAsync();

            return story;
        }

        public async Task SaveTransactionToDatabase(Transaction transaction)
        {
            var record = new ChangeEvent
            {
                BankAccId = transaction.AccountId,
                CreatedAt = transaction.CreatedAt,
                Type = transaction.Type,
                RelatedAccountId = transaction.RelatedAccountId,
                Amount = transaction.Amount,
                Currency = transaction.Currency
            };

            _CoreDbContext.ChangeEvents.Add(record);
            await _CoreDbContext.SaveChangesAsync();

            Update(transaction.AccountId);
        }

        private async void Update(Guid accountId) 
        {
            var transactions = await GetTransactions(accountId);
            var accountIdStr = accountId.ToString();
            await _hubContext.Clients.Group(accountIdStr)
                .SendAsync("ReceiveTransactions", transactions);
        }
    }
}
