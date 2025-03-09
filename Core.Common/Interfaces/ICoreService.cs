using Core.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Interfaces 
{
    public interface ICoreService
    {
        Task<BADto> CreateBankAcc(Guid clientId);
        Task<BADto> CloseBankAcc(Guid baId, Guid clientId);
        Task<BADto> ReplenishmentMoney(Guid Id, decimal Amount);
        Task<BADto> WithdrawMoney(Guid baId, Guid clientId, decimal Amount);
        Task<List<StoryDto>> BankAccStoryForClient(Guid baId, Guid clientId);


        Task<List<StoryDto>> BankAccStoryForEmployee(Guid baId);
        Task<List<BADto>> GetAllBankAccs(Guid clientId);

    }
}
