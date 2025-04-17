using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Account.DAL.Data.Entities;

namespace Account.DAL.Data;

/// <summary>
/// Auth database context
/// </summary>
public class AccountDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,
    UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{

    /// <summary>
    /// Users table
    /// </summary>
    public override DbSet<User> Users { get; set; }
    public override DbSet<Role> Roles { get; set; }
    public override DbSet<UserRole> UserRoles { get; set; }
    public DbSet<BirthDate> BirthDate { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasOne(u => u.BirthDate)
            .WithOne(c => c.User)
            .HasForeignKey<BirthDate>();

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserSettings)
            .WithOne(us => us.User)
            .HasForeignKey<UserSettings>(us => us.UserId); 

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>(o => {
            o.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            o.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });


    }

    /// <summary>
    /// Devices table
    /// </summary>
    public DbSet<Device> Devices { get; set; } = null!;

    /// <inheritdoc />
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {

    }
}


