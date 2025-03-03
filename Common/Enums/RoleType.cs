using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

/// <summary>
/// Role types
/// </summary>
public enum RoleType
{
    /// <summary>
    /// Administrator role
    /// </summary>
    [Display(Name = ApplicationRoleNames.Administrator)]
    Administrator,

    /// <summary>
    /// Default user role
    /// </summary>
    [Display(Name = ApplicationRoleNames.DefaultUser)]
    DefaultUser
}

/// <summary>
/// Role names
/// </summary>
public class ApplicationRoleNames
{
    /// <summary>
    /// Administrator role name
    /// </summary>
    public const string Administrator = "Administrator";

    /// <summary>
    /// Default user role name
    /// </summary>
    public const string DefaultUser = "DefaultUser";
}

