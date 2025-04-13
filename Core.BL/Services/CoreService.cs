using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Dtos;
using Core.Common.Interfaces;
using Core.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Exceptions;
using Core.Common.Enums;


namespace Core.BL.Services
{
    public class CoreService : ICoreService 
    {
        private readonly CoreDbContext _CoreDbContext;
        public CoreService(CoreDbContext coreDbContext)
        {
            _CoreDbContext = coreDbContext;
        }

        public async Task<BADto> CreateBankAcc(Guid clientId, Currency currency) {
            var bankAcc = new DAL.Entities.BankAccount
            {
                ClientId = clientId,
                Currency = currency,
                Amount = 0,
                CreatedAt = DateTime.UtcNow,
                Status = BankAccStatus.Open
            };

            _CoreDbContext.BankAccounts.Add(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            //_Producer.SendMessage(ChangeType.Create, bankAcc.Id);
            //AddRecord(Common.Enums.ChangeType.Create, bankAcc.Id);

            return new BADto
            {
                Id = bankAcc.Id,
                Currency = bankAcc.Currency,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<BADto> CloseBankAcc(Guid baId, Guid clientId)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            if(bankAcc.ClientId != clientId)
            {
                throw new ServiceUnavailableException("You do not have access to this bank account.");
            }

            bankAcc.Status = BankAccStatus.Close;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            //_Producer.SendMessage(ChangeType.Close, bankAcc.Id);
            //AddRecord(Common.Enums.ChangeType.Close, bankAcc.Id);

            return new BADto
            {
                Id = bankAcc.Id,
                Currency = bankAcc.Currency,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<List<BADto>> GetAllBankAccs(Guid clientId)
        {
            var bankAccs = await _CoreDbContext.BankAccounts
                .AsNoTracking()
                .Where(ba => ba.ClientId == clientId)
                .Select(ba => new BADto
                {
                    Id = ba.Id,
                    Amount = ba.Amount,
                    CreatedAt = ba.CreatedAt,
                    Status = ba.Status
                })
                .ToListAsync();

            return bankAccs;
        }

       

        /*private async void AddRecord(Common.Enums.ChangeType type, Guid baId)
        {
            var record = new DAL.Entities.ChangeEvent
            {
                BankAccId = baId,
                CreatedAt = DateTime.UtcNow,
                Type = type
            };

            _CoreDbContext.ChangeEvents.Add(record);

            await _CoreDbContext.SaveChangesAsync();
        }*/
    }
}
