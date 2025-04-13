using Core.Common.Dtos;
using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Interfaces
{
    public interface ITransactionService
    {
        Task<BADto> ReplenishmentMoney(Guid Id, decimal Amount, Currency currency);
        Task<BADto> WithdrawMoney(Guid baId, Guid clientId, decimal Amount, Currency currency);
        Task<BADto> TransferMoney(Guid clientId, Guid from, Guid to, decimal amount, Currency currency);
        Task<List<StoryDto>> GetTransactions(Guid baId);
        Task SaveTransactionToDatabase(Transaction transaction);
    }
}
