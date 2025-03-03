using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Common.DataTransferObjects;

/// <summary>
/// Token request DTO
/// </summary>
public class TokenRequestDto
{
    /// <summary>
    /// Expired access token
    /// </summary>
    [Required]
    [DisplayName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    [DisplayName("refresh_token")]
    public required string RefreshToken { get; set; }
}
