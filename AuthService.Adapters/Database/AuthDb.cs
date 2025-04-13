using AuthService.Adapters.Database.Models;
using AuthService.Adapters.Extensions;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using sltlang.Common.Common;
using sltlang.Common.Common.Extensions;
using System.ComponentModel;
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
                        Username = request.Login,
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

        public async Task<string?> GetUserVariable(int userId, Variable variable)
        {
            return await db.UserVariables.Where(x => x.UserId == userId && x.Name == variable).Select(x => x.Value).FirstOrDefaultAsync();
        }

        public async Task<Dictionary<Variable, string>> GetUserVariables(int userId, ICollection<Variable> variables)
        {
            var userVariables = await db.UserVariables.Where(x => x.UserId == userId && variables.Contains(x.Name)).ToDictionaryAsync(x => x.Name, x => x.Value!);
            var needDefault = variables.Where(x => !userVariables.ContainsKey(x));
            foreach (var variable in needDefault)
            {
                userVariables[variable] = EnumExtensions.EnumHelper<Variable>.AttributeHelper<DefaultValueAttribute>.With(variable, x => x?.Value?.SerializeByEnum(variable))!;
            }
            return userVariables;
        }

        public async Task<Dictionary<Permission, PermissionDto>> GetUserPermissions(int userId)
        {
            return await db.UserPermissions.Where(x => x.UserId == userId && (x.EndDate == null || x.EndDate > dateTime.UtcNow))
                .Select(x => x.ToPermissionDto()).ToDictionaryAsync(x => x.Permission, x => x);
        }

        
        private readonly Dictionary<SecurityLevel, HashSet<Variable>> securiyLevelVariables = EnumExtensions.GetSecurityLevelVariables();

        public async Task<CreateInviteLinkResponse> CreateInvite(InviteLinkDto request)
        {
            using (var tran = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await db.Invites.AnyAsync(x => x.Link == request.Link))
                        return new CreateInviteLinkResponse() { Result = CreateInviteResult.InviteAlreadyExists };

                    var permissions = await GetUserPermissions(request.UserId);

                    if (!permissions.ContainsKey(Permission.RootPermission) && (!permissions.ContainsKey(Permission.AuthInviteLinks)))
                        return new CreateInviteLinkResponse() { Result = CreateInviteResult.PermissionsError };

                    if (!permissions.ContainsKey(Permission.RootPermission) && (!permissions.ContainsKey(Permission.AuthInheritanceInviteLinks) && request.Permissions.Count > 0))
                        return new CreateInviteLinkResponse() { Result = CreateInviteResult.PermissionsError };

                    var variables = await GetUserVariables(request.UserId, [Variable.MaxLinkTTL, Variable.MaximumInviteLinks]);

                    if (!permissions.ContainsKey(Permission.RootPermission))
                        foreach (var securityLevel in EnumExtensions.EnumHelper<SecurityLevel>.Values)
                        {
                            if (request.Variables.Any(x => securiyLevelVariables[securityLevel].Contains(x.Key))
                                && !permissions.ContainsKey(Enum.Parse<Permission>($"AuthChanging{securityLevel}SecureVariables")))
                                return new CreateInviteLinkResponse() { Result = CreateInviteResult.PermissionsError };
                        }

                    var maxTtl = (int)variables[Variable.MaxLinkTTL].DeserializeByEnum(Variable.MaxLinkTTL)!;
                    var maxCount = (int)variables[Variable.MaximumInviteLinks].DeserializeByEnum(Variable.MaximumInviteLinks)!;

                    var inviteCount = await db.Invites.Where(x => x.UserId == request.UserId).CountAsync();

                    if (inviteCount >= maxCount)
                        return new CreateInviteLinkResponse() { Result = CreateInviteResult.TooManyInvites };

                    var ttl = (request.Ttl - dateTime.UtcNow).TotalMinutes;

                    if (ttl >= maxTtl)
                        return new CreateInviteLinkResponse() { Result = CreateInviteResult.PermissionsError };

                    var newInviteEntity = await db.Invites.AddAsync(new Invite()
                    {
                        InheritanceUserId = request.InheritanceUserId,
                        Ttl = request.Ttl,
                        Link = request.Link,
                        Permissions = request.Permissions.Select(x => x.ToConcretePermission<InvitePermission>()).ToArray(),
                        Variables = request.Variables.Select(x => x.ToConcreteVariable<InviteVariable>()).ToArray(),
                        UserId = request.UserId,
                    });

                    await db.SaveChangesAsync();
                    await tran.CommitAsync();
                    return new CreateInviteLinkResponse() { Result = CreateInviteResult.Success, InviteId = newInviteEntity.Entity.Id };
                }
                catch
                {
                    await tran.RollbackAsync();
                }
            }
            return new CreateInviteLinkResponse() { Result = CreateInviteResult.UnknownError };
        }
    }
}
