using Common.DataTransferObjects;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Controllers;

/// <summary>
/// User controller
/// </summary>
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    /// <summary>
    /// Get information about user's
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{userId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ProfileFullDto>> GetCurrentProfile()
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _userService.GetUserProfile(userId));
    }

    /// <summary> 
    /// Edit user's profile
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> UpdateProfile([FromBody] ProfileEditDto profileEditDto)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _userService.EditProfile(userId, profileEditDto);
        return Ok();
    }

    /// <summary> 
    /// Get user's metadata
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("metadata")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<string>> GetUserMetadata()
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _userService.GetUserMetadata(userId));
    }

    /// <summary> 
    /// Update user's metadata
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("metadata")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> UpdateUserMetadata([FromBody] string newMetadata)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _userService.UpdateUserMetadata(userId, newMetadata);
        return Ok();
    }

}

