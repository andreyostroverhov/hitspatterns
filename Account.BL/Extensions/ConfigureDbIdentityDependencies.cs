using Account.DAL.Data;
using Account.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Account.BL.Extensions;

/// <summary>
/// Extension methods for AccountAPI BL service Identity dependencies
/// </summary>
public static class ConfigureDbIdentityDependencies
{
    /// <summary>
    /// Add AccountAPI BL service Identity dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddIdentityManagers(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>()
            .AddRoleManager<RoleManager<Role>>();
        return services;
    }
}

