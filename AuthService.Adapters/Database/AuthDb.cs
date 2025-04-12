using AuthService.Adapters.Database.Models;
using AuthService.Adapters.Extensions;
using AuthService.Domain;
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

        public async Task<UserDto?> GetUserForAuth(string username, bool password = false)
        {
            var user = await db.Users.Include(x => x.Permissions).Include(x => x.Variables).AsNoTracking().FirstOrDefaultAsync(x => x.Username == username && x.Enabled);

            return user?.ToUserDto(password);
        }

        public async Task<UserDto?> GetUser(int userId, bool password = false)
        {
            var user = await db.Users.Include(x => x.Permissions).Include(x => x.Variables).Include(x => x.InvitedBy).Include(x => x.Invites).AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

            return user?.ToUserDto(password);
        }

        public async Task<bool> InviteExisted(string invite)
        {
            return await db.Invites.AnyAsync(x => x.Link == invite);
        }

        public async Task<RegistrationResponse?> RegisterUser(RegistrationRequest request)
        {
            var result = new RegistrationResponse();

            using (var tran = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var invite = await db.Invites
                        .Include(x => x.InheritanceUser).ThenInclude(x => x.Permissions)
                        .Include(x => x.InheritanceUser).ThenInclude(x => x.Variables)
                        .Include(x => x.Permissions)
                        .Include(x => x.Variables)
                        .FirstOrDefaultAsync(x => x.Link == request.Invite)
                        ?? throw new Exception("Invite not exists");

                    var permissions = (invite.InheritanceUser?.Permissions ?? []).ToList();
                    permissions.RemoveAll(x => invite.Permissions.Select(y => y.PermissionId).Contains(x.PermissionId));
                    foreach (var pemission in invite.Permissions)
                    {
                        permissions.Add(pemission.ToConcretePermission<UserPermission>());
                    }

                    var variables = (invite.InheritanceUser?.Variables ?? []).ToList();
                    variables.RemoveAll(x => invite.Variables.Select(y => y.Name).Contains(x.Name));
                    foreach (var variable in variables)
                    {
                        variables.Add(variable.ToConcreteVariable<UserVariable>());
                    }

                    var templateUser = invite.InheritanceUser?.IsTemplate ?? false ? invite.InheritanceUser.ToShortUserDto() : null;

                    var newUser = new User()
                    {
                        Username = request.Username,
                        PasswordHash = request.Password,
                        Enabled = true,
                        InvitedById = invite.UserId,
                        IsTemplate = false,
                        Variables = variables,
                        Permissions = permissions,
                    };

                    await db.Users.AddAsync(newUser);

                    db.Invites.Remove(invite);

                    await db.SaveChangesAsync();

                    await tran.CommitAsync();

                    return new RegistrationResponse()
                    {
                        User = newUser.ToUserDto(true),
                        TemplateUser = templateUser!,
                    };
                }
                catch
                {
                    await tran.RollbackAsync();
                    return null;
                }
            }
        }

        public Task<CreateInviteResult> CreateInvite(InviteLinkDto request)
        {
            throw new NotImplementedException();
        }
    }
}
