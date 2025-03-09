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
    public required string Email { get; set; }

    /// <summary>
    /// User`s password
    /// </summary>
    [Required]
    public required string Password { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }
}
