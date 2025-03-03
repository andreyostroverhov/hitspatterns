using Microsoft.AspNetCore.Identity;

namespace Account.DAL.Data.Entities;

public class UserRole : IdentityUserRole<Guid>
{
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}


