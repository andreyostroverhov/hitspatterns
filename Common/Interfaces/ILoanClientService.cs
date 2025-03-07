using Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Common.Interfaces;

public interface ILoanClientService
{
    Task<LoanDto> TakeLoanAsync(TakeLoanRequest request, Guid clientId);
    Task<RepayLoanResponse> RepayLoanAsync(RepayLoanRequest request);
    Task<List<TariffDto>> GetAvailableTariffsForClient();
    Task<List<LoanDto>> GetLoansClient(Guid clientId);
    Task<LoanDetailsDto> GetLoanDetailsClient(Guid loanId);
}