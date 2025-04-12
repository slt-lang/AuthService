using AuthService.Adapters.Database.Models;
using AuthService.Domain;
using AuthService.Domain.Extensions;
using AuthService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.Common.Extensions;
using System.Diagnostics;
using System.Xml.Linq;

namespace AuthService.Adapters.Database
{
    public class AuthDb(IDateTime dateTime, IMemoryCache memoryCache, AuthServiceContext db, Config config) : IAuthDb
    {
        public async Task<CreateUserResult> CreateUser(UserDto userDto)
        {
            //if (userDto.Invited)

            var user = new User()
            {
                Username = userDto.Username,
                Enabled = userDto.Enabled,
                IsTemplate = userDto.IsTemplate,
                PasswordHash = userDto.Password,
                InvitedById = null,
                Permissions = userDto.Permissions.Select(x => new UserPermission()
                {
                    PermissionId = x.Key,
                    AllowInheritance = x.Value.AllowInheritance,
                    EndDate = x.Value.EndDate,
                }).ToList(),
                Variables = userDto.Variables.Select(x => new UserVariable()
                {
                    Name = x.Key,
                    Value = x.Value.SerializeByEnum(x.Key)
                }).ToList(),
            };

            using (var tran = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await db.Users.AsNoTracking().AnyAsync(x => x.Username == user.Username))
                    {
                        return CreateUserResult.SameUsername;
                    }

                    await db.Users.AddAsync(user);
                    await db.SaveChangesAsync();

                    tran.Commit();

                    return CreateUserResult.Success;
                }
                catch
                {
                    tran.Rollback();
                }
            }

            return CreateUserResult.UnknownError;
        }

        public async Task<IList<UserDto>> GetTemplateUsers()
        {
            var templateUsers = await db.Users.Where(x => x.IsTemplate).Include(x => x.Permissions).Include(x => x.Variables).ToListAsync();
            return templateUsers.Select(x => x.ToUserDto()).ToArray();
        }

        public async Task<ShortUserDto?> GetShortUser(string username, bool password = false)
        {
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username && x.Enabled);

            return user?.ToShortUserDto(password);
        }

        public async Task<UserDto?> GetUser(string username, bool password = false)
        {
            var user = await db.Users.Include(x => x.Permissions).Include(x => x.Variables).Include(x => x.InvitedBy).AsNoTracking().FirstOrDefaultAsync(x => x.Username == username && x.Enabled);

            return user?.ToUserDto(password);
        }

        public async Task<UserDto?> GetUser(int userId, bool password = false)
        {
            var user = await db.Users.Include(x => x.Permissions).Include(x => x.Variables).Include(x => x.InvitedBy).AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

            return user?.ToUserDto(password);
        }
    }
}
