using Common.DataTransferObjects;

namespace Common.Interfaces;

public interface IUserService
{
    Task<ProfileFullDto> GetUserProfile(Guid userId);
    Task EditProfile(Guid userId, ProfileEditDto accountProfileEditDto);
    Task<string> GetUserMetadata(Guid userId);
    Task UpdateUserMetadata(Guid userId, string newMetadata);

}
