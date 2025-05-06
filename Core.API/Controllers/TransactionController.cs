using Common.Exceptions;
using Core.Common.Dtos;
using Core.Common.Enums;
using Core.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
[Route("api/transaction/")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _TransactionService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TransactionController(ITransactionService transactionService, IHttpContextAccessor httpContextAccessor)
    {
        _TransactionService = transactionService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Route("bank-account/{bankAccId}/replenish")]
    public async Task<ActionResult<IEnumerable<BADto>>> Replenish(Guid bankAccId, [FromBody] decimal Amount, Currency currency)
    {
        return Ok(await _TransactionService.ReplenishmentMoney(bankAccId, Amount, currency));
    }

    [HttpPost]
    [Route("bank-account/{bankAccId}/withdraw")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BADto>>> Withdraw(Guid bankAccId, [FromBody] decimal Amount, Currency currency)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _TransactionService.WithdrawMoney(bankAccId, clientId, Amount, currency));
    }

    [HttpPost]
    [Route("bank-account/transfer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BADto>>> Transfer(Guid from, Guid to, [FromBody] decimal amount, Currency currency)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _TransactionService.TransferMoney(clientId, from, to, amount, currency));
    }
}

