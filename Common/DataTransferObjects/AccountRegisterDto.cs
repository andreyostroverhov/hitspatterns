using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Common.DataTransferObjects;

/// <summary>
/// DTO for user registration
/// </summary>
public class AccountRegisterDto
{
    /// <summary>
    /// User`s email
    /// </summary>
    [Required]
    [EmailAddress]
    [DisplayName("email")]
    public required string Email { get; set; }

    /// <summary>
    /// User`s password
    /// </summary>
    [Required]
    [DefaultValue("P@ssw0rd")]
    [DisplayName("password")]
    [MinLength(8)]
    public required string Password { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public DateTime? BirthDate { get; set; }
}
