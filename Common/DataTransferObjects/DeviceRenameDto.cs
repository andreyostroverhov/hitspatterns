using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

/// <summary>
/// DTO for device rename
/// </summary>
public class DeviceRenameDto
{
    /// <summary>
    /// Device name
    /// </summary>
    [Required]
    public required string DeviceName { get; set; }
}
