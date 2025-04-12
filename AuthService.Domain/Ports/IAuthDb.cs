using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;

namespace AuthService.Domain.Ports
{
    public interface IAuthDb
    {
        Task<CreateUserResult> CreateUser(UserDto userDto);
        Task<UserDto?> GetUser(int userId, bool password = false);
        Task<ShortUserDto?> GetShortUser(string username, bool password = false);
        Task<IList<UserDto>> GetTemplateUsers();
        Task<UserDto?> GetUserForAuth(string username, bool password = false);
        Task<bool> InviteExisted(string invite);
        Task<RegistrationResponse?> RegisterUser(RegistrationRequest request);
        Task<CreateInviteResult> CreateInvite(InviteLinkDto request);
    }
}
