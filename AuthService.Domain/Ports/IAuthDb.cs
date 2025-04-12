using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;

namespace AuthService.Domain.Ports
{
    public interface IAuthDb
    {
        Task<CreateUserResult> CreateUser(UserDto userDto);
        Task<UserDto?> GetUser(int userId, bool password = false);
        Task<UserDto?> GetUser(string username, bool password = false);
        Task<ShortUserDto?> GetShortUser(string username, bool password = false);
        Task<IList<UserDto>> GetTemplateUsers();
    }
}
