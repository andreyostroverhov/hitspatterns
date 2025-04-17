using Common.DataTransferObjects;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loan.API.Controllers;

/// <summary>
/// Loan employee controller
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Employee + "," + ApplicationRoleNames.Administrator)]
[Route("api/loan-employee")]
public class LoanEmployeeController : ControllerBase
{
    private readonly ILoanEmployeeService _loanEmployeeService;
    private readonly ILoanClientService _loanClientService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="loanEmployeeService"></param>
    public LoanEmployeeController(ILoanEmployeeService loanEmployeeService, ILoanClientService loanClientService)
    {
        _loanEmployeeService = loanEmployeeService;
        _loanClientService = loanClientService;
    }
    /// <summary>
    /// Get available tariffs (Получить список всех тарифов)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("tariff")]
    public async Task<ActionResult<List<TariffDto>>> GetTariffList()
    {
        return Ok(await _loanEmployeeService.GetTariffsAsync());
    }

    /// <summary>
    /// Create new tariff (Создать новый тариф)
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("tariff")]
    public async Task<ActionResult<TariffDto>> CreateTariff([FromBody] CreateTariffRequest request)
    {
        return Ok(await _loanEmployeeService.CreateTariffAsync(request));
    }

    /// <summary>
    /// Delete tariff (Удалить тариф)
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("tariff/{tariffId}")]
    public async Task<ActionResult> DeleteTariff(Guid tariffId)
    {
        await _loanEmployeeService.DeleteTariffAsync(tariffId);
        return Ok(new { message = "Тариф успешно удален." });
    }

    /// <summary>
    /// Get client's loans (Получить список кредитов клиента)
    /// </summary>
    /// <returns></returns>
    [HttpGet("clients/{clientId}/loans")]
    public async Task<ActionResult<List<LoanDto>>> GetClientLoans(Guid clientId)
    {
        return Ok(await _loanEmployeeService.GetClientLoansAsync(clientId));
    }

    /// <summary>
    /// Get detailed information about loan (Получить детальную информацию о кредите)
    /// </summary>
    /// <returns></returns>
    [HttpGet("loans/{loanId}")]
    public async Task<ActionResult<LoanDetailsDto>> GetLoanDetails(Guid loanId)
    {
        return Ok(await _loanEmployeeService.GetLoanDetailsAsync(loanId));
    }


    [HttpGet("clients/{clientId}/overdue-payments")]
    public async Task<ActionResult<List<LoanScheduleDto>>> GetClientOverduePayments(Guid clientId)
    {
        return Ok(await _loanClientService.GetOverduePayments(clientId));
    }

    [HttpGet("clients/{clientId}/credit-rating")]
    public async Task<ActionResult<int>> GetClientCreditRating(Guid clientId)
    {
        return Ok(await _loanClientService.GetCreditRating(clientId));
    }
}
