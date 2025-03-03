using Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace Account.DAL.Data.Entities;

public class Role : IdentityRole<Guid>
{
    public RoleType RoleType { get; set; }
    public ICollection<UserRole> Users { get; set; }
}

