using System.ComponentModel;

namespace Common.DataTransferObjects;

/// <summary>
/// Token response DTO
/// </summary>
public class TokenResponseDto
{
    /// <summary>
    /// Access token
    /// </summary>
    [DisplayName("accessToken")]
    public required string accessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [DisplayName("refreshToken")]
    public required string refreshToken { get; set; }

    /// <summary>
    /// User id
    /// </summary>
    [DisplayName("userId")]
    public required string userId { get; set; }
}
