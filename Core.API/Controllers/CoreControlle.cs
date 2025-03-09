using Common.Exceptions;
using Core.Common.Dtos;
using Core.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
[Route("api/core/")]
public class CoreController : ControllerBase
{
    private readonly ICoreService _CoreService;

    public CoreController(ICoreService CoreService)
    {
        _CoreService = CoreService;
    }

    [HttpPost]
    [Route("bank-account/create")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BADto>>> CreateBankAcc()
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.CreateBankAcc(clientId));
    }

    [HttpPost]
    [Route("bank-account/{bankAccId}/close")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BADto>>> CloseBankAcc(Guid bankAccId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.CloseBankAcc(bankAccId, clientId));
    }

    [HttpPost]
    [Route("bank-account/{bankAccId}/replenish")]
    public async Task<ActionResult<IEnumerable<BADto>>> Replenish(Guid bankAccId, [FromBody] decimal Amount)
    {
        return Ok(await _CoreService.ReplenishmentMoney(bankAccId, Amount));
    }

    [HttpPost]
    [Route("bank-account/{bankAccId}/withdraw")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<IEnumerable<BADto>>> Withdraw(Guid bankAccId, [FromBody] decimal Amount)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.WithdrawMoney(bankAccId, clientId, Amount));
    }

    [HttpGet]
    [Route("bank-account/{bankAccId}/story")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<List<StoryDto>>> StoryForClient(Guid bankAccId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.BankAccStoryForClient(bankAccId, clientId));
    }

    [HttpGet]
    [Route("bank-account/{bankAccId}/story-for-emploee")]
    public async Task<ActionResult<List<StoryDto>>> StoryForEmploee(Guid bankAccId)
    {
        return Ok(await _CoreService.BankAccStoryForEmployee(bankAccId));
    }

    [HttpGet]
    [Route("bank-accounts/all")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<List<BADto>>> AllBankAccsForUser()
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.GetAllBankAccs(clientId));
    }

    [HttpPost]
    [Route("bank-accounts/all-for-emploee")]
    public async Task<ActionResult<List<BADto>>> AllBankAccsForEmploee([FromBody] Guid clientId)
    {
        return Ok(await _CoreService.GetAllBankAccs(clientId));
    }

}

