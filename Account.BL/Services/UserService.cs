using Account.DAL.Data;
using Account.DAL.Data.Entities;
using Common.DataTransferObjects;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Common.Other;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Account.BL.Services;

/// <inheritdoc cref="IUserService"/>
public class UserService : IUserService
{

    private readonly UserManager<User> _userManager;
    private readonly AccountDbContext _accountDbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="accountDbContext"></param>
    public UserService(UserManager<User> userManager, AccountDbContext accountDbContext)
    {
        _userManager = userManager;
        _accountDbContext = accountDbContext;
    }

    public async Task<ProfileFullDto> GetUserProfile(Guid userId)
    {
        var userM = await _userManager.FindByIdAsync(userId.ToString());
        var user = await _accountDbContext.Users
            .Include(u => u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var profile = new ProfileFullDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            BirthDate = new BirthDateDto
            {
                Value = user.BirthDate.Value,
            },
            JoinedAt = user.JoinedAt,
            Roles = await _userManager.GetRolesAsync(userM!),
        };
        return profile;
    }

    public async Task EditProfile(Guid userId, ProfileEditDto accountProfileEditDto)
    {
        var user = await _accountDbContext.Users
            .Include(u => u.BirthDate)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        user.FullName = accountProfileEditDto.FullName;
        user.BirthDate.Value = accountProfileEditDto.BirthDate;

        _accountDbContext.UpdateRange(user);
        await _accountDbContext.SaveChangesAsync();
    }

    public async Task<string> GetUserMetadata(Guid userId)
    {
        var user = await _accountDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        var response = user.Metadata;
        if (response == null)
        {
            return " ";
        }
        return response;
    }

    public async Task UpdateUserMetadata(Guid userId, string newMetadata)
    {
        var user = await _accountDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        user.Metadata = newMetadata;
        _accountDbContext.UpdateRange(user);
        await _accountDbContext.SaveChangesAsync();
    }

    public async Task<List<UserShortDto>> GetUsers()
    {
        var users1 = _userManager.Users
            .AsNoTracking();


        var shortUsers = users1.Select(user => new UserShortDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = user.Roles.Any(r => r.Role.RoleType == RoleType.Administrator)
                ? ApplicationRoleNames.Administrator
                : user.Roles.Any(r => r.Role.RoleType == RoleType.Employee)
                    ? ApplicationRoleNames.Employee
                    : user.Roles.Any(r => r.Role.RoleType == RoleType.DefaultUser)
                        ? ApplicationRoleNames.DefaultUser
                        : null
        }).ToList();

        return shortUsers;
    }
}

