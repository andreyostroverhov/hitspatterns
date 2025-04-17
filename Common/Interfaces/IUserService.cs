using Common.DataTransferObjects;
using Common.Other;

namespace Common.Interfaces;

public interface IUserService
{
    Task<ProfileFullDto> GetUserProfile(Guid userId);
    Task EditProfile(Guid userId, ProfileEditDto accountProfileEditDto);
    Task<UserSettings> GetSettings(Guid userId);
    Task EditUserSettings(Guid userId, UserSettings userSettings);
    Task<string> GetUserMetadata(Guid userId);
    Task UpdateUserMetadata(Guid userId, string newMetadata);

    public Task<List<UserShortDto>> GetUsers();

}
