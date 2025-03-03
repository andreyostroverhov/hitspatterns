using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataTransferObjects;

/// <summary>
/// Data transfer object for changing password
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Old password
    /// </summary>
    [Required]
    [DisplayName("old_password")]
    public required string OldPassword { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [DisplayName("new_password")]
    public required string NewPassword { get; set; }
}
