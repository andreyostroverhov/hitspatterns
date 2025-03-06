using Common.DataTransferObjects;

namespace Common.Interfaces;

public interface ILoanEmployeeService
{
    //Методы тарифа
    Task<List<TariffDto>> GetTariffsAsync();
    Task<TariffDto> CreateTariffAsync(CreateTariffRequest request);
    Task DeleteTariffAsync(Guid id);


    //Методы по кредитам
    Task<List<LoanDto>> GetClientLoansAsync(Guid clientId);
    Task<LoanDetailsDto> GetLoanDetailsAsync(Guid loanId);
}
