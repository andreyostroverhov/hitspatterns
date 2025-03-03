using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

/// <summary>
/// Profile DTO for Edit
/// </summary>
public class ProfileEditDto
{


    /// <summary>
    /// User`s full name (surname, name, patronymic)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }

}
