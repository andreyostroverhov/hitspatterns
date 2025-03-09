using Common.DataTransferObjects;
using Microsoft.AspNetCore.Http;

namespace Common.Interfaces;

public interface IAuthService
{
    Task<AccountRegisterDto> RegisterAsync(AccountRegisterDto accountRegisterDto, HttpContext httpContext);
    Task<TokenResponseDto> LoginAsync(AccountLoginDto accountLoginDto, HttpContext httpContext);
    Task LogoutAsync(Guid userId, HttpContext httpContext);
    Task<TokenResponseDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto, HttpContext httpContext);
    Task<List<DeviceDto>> GetDevicesAsync(Guid userId);
    Task RenameDeviceAsync(Guid userId, Guid deviceId, DeviceRenameDto deviceRenameDto);
    Task DeleteDeviceAsync(Guid userId, Guid deviceId);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
    Task ResetPasswordAsync(ResetPasswordDto model);
    Task UnbanUser(Guid userId);
    Task BanUser(Guid userId);

}

