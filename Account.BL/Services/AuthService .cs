using Account.DAL.Data;
using Account.DAL.Data.Entities;
using Common.DataTransferObjects;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Account.BL.Services;


/// <summary>
/// Service for authentication and authorization
/// </summary>
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly AccountDbContext _accountDbContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="accountDbContext"></param>
    /// <param name="configuration"></param>
    //   /// <param name="emailService"></param>
    public AuthService(ILogger<AuthService> logger, UserManager<User> userManager, SignInManager<User> signInManager,
        AccountDbContext accountDbContext, IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _accountDbContext = accountDbContext;
        _configuration = configuration;

    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="accountRegisterDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<AccountRegisterDto> RegisterAsync(AccountRegisterDto accountRegisterDto, HttpContext httpContext)
    {
        if (accountRegisterDto.Email == null)
        {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Email is empty");
        }

        if (accountRegisterDto.Password == null)
        {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Password is empty");
        }

        if (accountRegisterDto.FullName == null)
        {
            throw new ArgumentNullException(nameof(accountRegisterDto), "Name is empty");
        }

        if (await _userManager.FindByEmailAsync(accountRegisterDto.Email) != null)
        {
            throw new ConflictException("User with this email already exists");
        }

        var user = new User
        {
            Email = accountRegisterDto.Email,
            UserName = accountRegisterDto.Email,
            FullName = accountRegisterDto.FullName,
            DeviceToken = accountRegisterDto.DeviceToken
        };
        user.BirthDate = new BirthDate
        {
            Value = accountRegisterDto.BirthDate,
            User = user
        };
        var result = await _userManager.CreateAsync(user, accountRegisterDto.Password);

        if (result.Succeeded)
        {
            var newUser = await _userManager.FindByIdAsync(user.Id.ToString())
                ?? throw new ConflictException("User not created.");

            await _userManager.AddToRoleAsync(newUser, ApplicationRoleNames.DefaultUser);
            _logger.LogInformation("Successful register");
            return new AccountRegisterDto()
            {
                Email = accountRegisterDto.Email,
                Password = accountRegisterDto.Password
            };
        }
        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        throw new BadRequestException(errors);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="accountLoginDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> LoginAsync(AccountLoginDto accountLoginDto, HttpContext httpContext)
    {
        var identity = await GetIdentity(accountLoginDto.Email.ToLower(), accountLoginDto.Password) ?? throw new BadRequestException("Incorrect username or password or user is banned");
        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(x => x.Email == accountLoginDto.Email) ?? throw new NotFoundException("User not found");
        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UnauthorizedException("User is banned");
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent);

        if (device == null)
        {
            device = new Device()
            {
                User = user,
                RefreshToken = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
                UserAgent = userAgent,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };
            await _accountDbContext.Devices.AddAsync(device);
        }

        device.LastActivity = DateTime.UtcNow;
        device.ExpirationDate = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt")
            .GetValue<int>("RefreshTokenLifetimeInDays"));

        await _accountDbContext.SaveChangesAsync();
        var claims = new List<Claim>(identity.Claims);
        claims.Add(new Claim("deviceToken", user.DeviceToken));

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: _configuration.GetSection("Jwt")["Audience"],
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        _logger.LogInformation("Successful login");

        return new TokenResponseDto()
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            refreshToken = device.RefreshToken,
            userId = user.Id.ToString(),
        };
    }

    /// <summary>
    /// Logout user by deleting his current device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task LogoutAsync(Guid userId, HttpContext httpContext)
    {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(x => x.Id == userId) ?? throw new NotFoundException("User not found");
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent) ?? throw new MethodNotAllowedException("You can`t logout from this device");
        _accountDbContext.Devices.Remove(device);
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="tokenRequestDto"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto, HttpContext httpContext)
    {
        tokenRequestDto.AccessToken = tokenRequestDto.AccessToken.Replace("Bearer ", "");
        var principal = GetPrincipalFromExpiredToken(tokenRequestDto.AccessToken);
        if (principal.Identity == null)
        {
            throw new BadRequestException("Invalid jwt token");
        }

        var user = _userManager.Users.Include(x => x.Devices)
            .FirstOrDefault(x => x.Id.ToString() == principal.Identity.Name) ?? throw new NotFoundException("User not found");
        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UnauthorizedException("User is banned");
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

        var device =
            user.Devices.FirstOrDefault(x => x.IpAddress == ipAddress && x.UserAgent == userAgent)
            ?? throw new MethodNotAllowedException("You can't refresh token from another device. Re-login needed");
        if (device.RefreshToken != tokenRequestDto.RefreshToken)
        {
            throw new BadRequestException("Refresh token is invalid");
        }

        if (device.ExpirationDate < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token is expired. Re-login needed");
        }

        var jwt = new JwtSecurityToken(
            issuer: _configuration.GetSection("Jwt")["Issuer"],
            audience: null,
            notBefore: DateTime.UtcNow,
            claims: principal.Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_configuration.GetSection("Jwt")
                .GetValue<int>("AccessTokenLifetimeInMinutes"))),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256));

        device.LastActivity = DateTime.UtcNow;
        device.ExpirationDate = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt")
            .GetValue<int>("RefreshTokenLifetimeInDays"));
        await _accountDbContext.SaveChangesAsync();

        return new TokenResponseDto()
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            refreshToken = device.RefreshToken,
            userId = user.Id.ToString()
        };
    }

    /// <summary>
    /// Get user devices
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<DeviceDto>> GetDevicesAsync(Guid userId)
    {
        var user = _userManager.Users.Include(x => x.Devices).FirstOrDefault(u => u.Id == userId);
        return user == null
            ? throw new NotFoundException("User not found")
            : Task.FromResult(user.Devices.Select(d => new DeviceDto
            {
                DeviceName = d.DeviceName,
                IpAddress = d.IpAddress,
                UserAgent = d.UserAgent,
                LastActivity = d.LastActivity,
                Id = d.Id,
            }).ToList());
    }

    /// <summary>
    /// Rename device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="deviceRenameDto"></param>
    /// <returns></returns>
    public async Task RenameDeviceAsync(Guid userId, Guid deviceId, DeviceRenameDto deviceRenameDto)
    {
        var user = _userManager.Users
            .Include(x => x.Devices)
            .FirstOrDefault(u => u.Id == userId) ?? throw new NotFoundException("User not found");
        var device = user.Devices.FirstOrDefault(d => d.Id == deviceId) ?? throw new NotFoundException("Device not found");
        device.DeviceName = deviceRenameDto.DeviceName;
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Delete device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    public async Task DeleteDeviceAsync(Guid userId, Guid deviceId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId) ?? throw new NotFoundException("User not found");
        var device = _accountDbContext.Devices.FirstOrDefault(d => d.User == user) ?? throw new NotFoundException("Device not found");
        _accountDbContext.Devices.Remove(device);
        await _accountDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="changePasswordDto"></param>
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId) ?? throw new NotFoundException("User not found");
        var result =
            await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="model"></param>
    public async Task ResetPasswordAsync(ResetPasswordDto model)
    {
        var userM = await _userManager.FindByEmailAsync(model.Email) ?? throw new NotFoundException("User was not found.");
        var result =
            await _userManager.ResetPasswordAsync(userM, HttpUtility.UrlDecode(model.Token), model.NewPassword);
        if (!result.Succeeded)
        {
            throw new ConflictException(string.Join(", ", result.Errors.Select(x => x.Description)));
        };
    }
    public async Task ConfirmEmail(Guid userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found");
        var result =
            await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string jwtToken)
    {
        var key = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"] ?? string.Empty));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _configuration.GetSection("Jwt")["Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration.GetSection("Jwt")["Audience"],
            ValidateLifetime = false
        };

        ClaimsPrincipal principal;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException("Invalid jwt token", ex);
        }

        return principal;
    }

    private async Task<ClaimsIdentity?> GetIdentity(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return null;

        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Id.ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims, "Token", ClaimTypes.Name, ClaimTypes.Role);
    }

    public async Task BanUser(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");

        if (userM.LockoutEnabled)
        {
            DateTimeOffset lockoutDate = new DateTimeOffset(DateTime.UtcNow.AddYears(50));
            await _userManager.SetLockoutEndDateAsync(userM, lockoutDate);
            await _userManager.UpdateAsync(userM);
        }
        else
        {
            throw new BadRequestException("User cannot be blocked.");
        }
    }

    public async Task UnbanUser(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User with this Id not found.");

        if (userM.LockoutEnabled)
        {
            await _userManager.SetLockoutEndDateAsync(userM, null);
            await _userManager.UpdateAsync(userM);
        }
        else
        {
            throw new BadRequestException("User cannot be unblocked.");
        }
    }
}

