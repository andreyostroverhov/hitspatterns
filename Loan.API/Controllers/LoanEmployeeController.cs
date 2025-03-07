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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="loanEmployeeService"></param>
    public LoanEmployeeController(ILoanEmployeeService loanEmployeeService)
    {
        _loanEmployeeService = loanEmployeeService;
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
    [Route("tariff")]
    public async Task<ActionResult> DeleteTariff([FromBody] DeleteTariffRequest request)
    {
        await _loanEmployeeService.DeleteTariffAsync(request.Id);
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

}
