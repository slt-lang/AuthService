using AuthService.Adapters.Database.Models;
using AuthService.Domain;
using AuthService.Domain.Extensions;
using AuthService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sltlang.Common.AuthService.Dto;

namespace AuthService.Adapters.Database
{
    public class AuthDb(IDateTime dateTime, IMemoryCache memoryCache, AuthServiceContext db, Config config) : IAuthDb
    {
        public async Task<UserDto?> AllowAuthorization(string username, string passwordHash)
        {
            //var user = await db.Users.FirstOrDefaultAsync(x => x.Username == username && x.PasswordHash == passwordHash && x.Enabled);

            var user = new User()
            {
                Id = 0,
                Username = "root",
                Permissions =
                [
                    new(){PermissionId = sltlang.Common.AuthService.Enums.Permission.RootPermission}
                ],
                PasswordHash = "x",
                Enabled = true,
            };

            if (user.Username == username && user.PasswordHash == passwordHash && user.Enabled)
                return user.ToUserDto();

            if (user == null)
                return null;

            return user.ToUserDto();
        }

        public Task<UserDto[]> GetTemplateUsers(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<InviteLinkDto> CreateInviteLink(int userId, InviteLinkDto linkDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasInviteLink(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteInviteLink(int userId, string path)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
