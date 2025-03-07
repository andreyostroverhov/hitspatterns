using Common.DataTransferObjects;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Loan.BL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Loan.API.Controllers;

/// <summary>
/// Loan client controller
/// </summary>
[ApiController]
[Route("api/loan-client")]
public class LoanClientController : ControllerBase
{
    private readonly ILoanClientService _loanClientService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="loanClientService"></param>
    public LoanClientController(ILoanClientService loanClientService)
    {
        _loanClientService = loanClientService;
    }

    /// <summary>
    /// Get available tariffs for client (Получить список доступных тарифов) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("tariff")]
    public async Task<ActionResult<List<TariffDto>>> GetTariffList()
    {
        return Ok(await _loanClientService.GetAvailableTariffsForClient());
    }


    /// <summary>
    /// Get currencies (Получить список валют) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpGet("currencies")]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrenciesAsync()
    {
        // Имитация асинхронной операции (например, если в будущем потребуется обращение к БД)
        var currencies = await Task.Run(() => Enum.GetValues(typeof(Currency)).Cast<Currency>().ToList());
        return Ok(currencies);
    }

    /// <summary>
    /// Get clients loans (Получить список кредитов пользователя) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("loans")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<List<LoanDto>>> GetLoansClient()
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _loanClientService.GetLoansClient(clientId));
    }

    /// <summary>
    /// Get detailed information about loan (Получить детальную информацию о кредите) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpGet("loans/{loanId}")]
    public async Task<ActionResult<LoanDetailsDto>> GetLoanDetailsClient(Guid loanId)
    {
        return Ok(await _loanClientService.GetLoanDetailsClient(loanId));
    }

    /// <summary>
    /// Get new loan (Взять кредит) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpPost("take-new")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<LoanDto>> GetNewLoan([FromBody] TakeLoanRequest request)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        var loan = await _loanClientService.TakeLoanAsync(request, clientId);
        return Ok(loan);
    }


    /// <summary>
    /// Repay loan (Погасить кредит полностью или частично) [Client]
    /// </summary>
    /// <returns></returns>
    [HttpPost("repay")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<RepayLoanResponse>> RepayLoan([FromBody] RepayLoanRequest request)
    {
        var result = await _loanClientService.RepayLoanAsync(request);
        return Ok(result);
    }
}