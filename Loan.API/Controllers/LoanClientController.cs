using Common.DataTransferObjects;
using Common.Interfaces;
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
    /// Get new loan (Взять кредит)
    /// </summary>
    /// <returns></returns>
    [HttpPost("take-new")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<LoanDto>> GetNewLoan([FromBody] TakeLoanRequest request)
    {
        var loan = await _loanClientService.TakeLoanAsync(request);
        return Ok(loan);
    }


    /// <summary>
    /// Repay loan (Погасить кредит полностью или частично)
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