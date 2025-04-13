using Common.Exceptions;
using Core.Common.Dtos;
using Core.Common.Enums;
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
    public async Task<ActionResult<IEnumerable<BADto>>> CreateBankAcc(Currency currency)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid clientId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _CoreService.CreateBankAcc(clientId, currency));
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