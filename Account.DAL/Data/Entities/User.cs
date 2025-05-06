﻿using Microsoft.AspNetCore.Identity;

namespace Account.DAL.Data.Entities;

/// <summary>
/// AccountDb general User Model   
/// </summary>
public class User : IdentityUser<Guid>
{
    /// <summary>
    /// User`s full name
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// User`s birth date
    /// </summary>
    public BirthDate? BirthDate { get; set; }

    /// <summary>
    /// User roles  
    /// </summary>
    public ICollection<UserRole>? Roles { get; set; }
    /// <summary>
    /// User's post
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User`s devices
    /// </summary>
    public List<Device> Devices { get; set; } = new List<Device>();

    /// <summary>
    /// User`s metadata
    /// </summary>
    public string? Metadata { get; set; }

    public string? DeviceToken { get; set; }
}

