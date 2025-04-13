using Core.Common.Dtos;
using Core.Common.Enums;
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
        Task<BADto> CreateBankAcc(Guid clientId, Currency currency);
        Task<BADto> CloseBankAcc(Guid baId, Guid clientId);
        Task<List<BADto>> GetAllBankAccs(Guid clientId);
    }
}
