using Common.DataTransferObjects;
using System.Threading.Tasks;

namespace Common.Interfaces;

public interface ILoanClientService
{
    Task<LoanDto> TakeLoanAsync(TakeLoanRequest request);
    Task<RepayLoanResponse> RepayLoanAsync(RepayLoanRequest request);
}