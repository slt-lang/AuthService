using sltlang.Common.AuthService.Dto;

namespace AuthService.Domain.Ports
{
    public interface IAuthDb
    {
        public Task<UserDto[]> GetTemplateUsers(int userId);
        public Task<InviteLinkDto> CreateInviteLink(int userId, InviteLinkDto linkDto);
        public Task<bool> HasInviteLink(string path);
        public Task<bool> DeleteInviteLink(int userId, string path);
        public Task<UserDto> GetUser(string username);
        public Task<UserDto> GetUser(int userId);
    }
}
