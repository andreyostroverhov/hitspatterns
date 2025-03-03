namespace Common.DataTransferObjects;

/// <summary>
/// User profile DTO
/// </summary>
public class ProfileFullDto
{
    /// <summary>
    /// Profile Identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public BirthDateDto BirthDate { get; set; }

    /// <summary>
    /// Date when user joined the system
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// User's roles
    /// </summary>
    public IList<string> Roles { get; set; }
}
