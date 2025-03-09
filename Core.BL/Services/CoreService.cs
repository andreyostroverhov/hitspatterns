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


namespace Core.BL.Services
{
    public class CoreService : ICoreService 
    {
        private readonly CoreDbContext _CoreDbContext;
        public CoreService(CoreDbContext coreDbContext)
        {
            _CoreDbContext = coreDbContext;
        }

        public async Task<BADto> CreateBankAcc(Guid clientId) {
            var bankAcc = new DAL.Entities.BankAccount
            {
                ClientId = clientId,
                Amount = 0,
                CreatedAt = DateTime.UtcNow,
                Status = Common.Enums.BankAccStatus.Open
            };

            _CoreDbContext.BankAccounts.Add(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            AddRecord(Common.Enums.ChangeType.Create, bankAcc.Id);

            return new BADto
            {
                Id = bankAcc.Id,
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

            bankAcc.Status = Common.Enums.BankAccStatus.Close;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            AddRecord(Common.Enums.ChangeType.Close, bankAcc.Id);

            return new BADto
            {
                Id = bankAcc.Id,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<BADto> ReplenishmentMoney(Guid Id, decimal Amount)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == Id) ?? throw new NotFoundException("Bank account not found.");

            bankAcc.Amount = bankAcc.Amount + Amount;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            AddRecord(Common.Enums.ChangeType.Replenishment, bankAcc.Id); 

            return new BADto
            {
                Id = bankAcc.Id,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<BADto> WithdrawMoney(Guid baId, Guid clientId, decimal Amount)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            if (bankAcc.ClientId != clientId)
            {
                throw new ServiceUnavailableException("You do not have access to this bank account.");
            }

            if (bankAcc.Amount < Amount){
                throw new ForbiddenException("Тot enough funds");
            }

            bankAcc.Amount = bankAcc.Amount - Amount;
            _CoreDbContext.BankAccounts.UpdateRange(bankAcc);
            await _CoreDbContext.SaveChangesAsync();

            AddRecord(Common.Enums.ChangeType.Withdraw, bankAcc.Id);
           
            return new BADto
            {
                Id = bankAcc.Id,
                Amount = bankAcc.Amount,
                CreatedAt = bankAcc.CreatedAt,
                Status = bankAcc.Status
            };
        }

        public async Task<List<StoryDto>> BankAccStoryForClient(Guid baId, Guid clientId)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
               .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            if (bankAcc.ClientId != clientId)
            {
                throw new ServiceUnavailableException("You do not have access to this bank account.");
            }

            var story = await _CoreDbContext.ChangeEvents
                .AsNoTracking()
                .Where(ce => ce.BankAccId == baId)
                .Select(ce => new StoryDto
                {
                    Id = ce.Id,
                    CreatedAt = ce.CreatedAt,
                    Type = ce.Type
                })
                .ToListAsync();

            return story;
        }

        public async Task<List<StoryDto>> BankAccStoryForEmployee(Guid baId)
        {
            var bankAcc = await _CoreDbContext.BankAccounts
                .FirstOrDefaultAsync(ba => ba.Id == baId) ?? throw new NotFoundException("Bank account not found.");

            var story = await _CoreDbContext.ChangeEvents
               .AsNoTracking()
               .Where(ce => ce.BankAccId == baId)
               .Select(ce => new StoryDto
               {
                   Id = ce.Id,
                   CreatedAt = ce.CreatedAt,
                   Type = ce.Type
               })
               .ToListAsync();

            return story;
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

       

        private async void AddRecord(Common.Enums.ChangeType type, Guid baId)
        {
            var record = new DAL.Entities.ChangeEvent
            {
                BankAccId = baId,
                CreatedAt = DateTime.UtcNow,
                Type = type
            };

            _CoreDbContext.ChangeEvents.Add(record);

            await _CoreDbContext.SaveChangesAsync();
        }
    }
}
