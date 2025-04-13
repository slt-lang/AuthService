using AuthService.Adapters.Database.Models;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using sltlang.Common.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using sltlang.Common.Common.Extensions;

namespace AuthService.Adapters.Extensions
{
    public static class DtoExtensions
    {
        public static ShortUserDto ToShortUserDto(this User user, bool password = false)
        {
            return new ShortUserDto()
            {
                Enabled = user.Enabled,
                Id = user.Id,
                IsTemplate = user.IsTemplate,
                Password = password ? user.PasswordHash : null,
                Username = user.Username,
            };
        }

        public static UserDto ToUserDto(this User user, bool password = false)
        {
            return new UserDto()
            {
                Enabled = user.Enabled,
                Id = user.Id,
                IsTemplate = user.IsTemplate,
                Password = password ? user.PasswordHash : null,
                Permissions = user.Permissions?.ToDictionary(x => x.PermissionId, x => x.ToPermissionDto())!,
                RegistrationDate = user.RegisterDate.ToLocalTime(),
                Username = user.Username,
                Variables = user.Variables?.ToDictionary(x => x.Name, x => x.Value)!,
                InvitedBy = user.ToShortUserDto(),
                InviteLinks = user.Invites?.Select(x => x.ToShortInviteLinkDto()).ToArray()!,
            };
        }

        public static PermissionDto ToPermissionDto(this AbstractPermission permission)
        {
            return new PermissionDto()
            {
                CreateDate = permission.CreateDate.ToLocalTime(),
                EndDate = permission.EndDate?.ToLocalTime(),
                Id = permission.Id,
                AllowInheritance = permission.AllowInheritance,
                Permission = permission.PermissionId,
            };
        }

        public static ShortInviteLinkDto ToShortInviteLinkDto(this Invite invite)
        {
            return new ShortInviteLinkDto()
            {
                InheritanceUserId = invite.InheritanceUserId,
                TemplateUsername = invite.InheritanceUser?.Username,
                Link = invite.Link,
                LinkId = invite.Id,
                Ttl = invite.Ttl.ToLocalTime(),
                UserId = invite.UserId,
            };
        }

        public static InviteLinkDto ToInviteLinkDto(this Invite invite)
        {
            return new InviteLinkDto()
            {
                InheritanceUserId = invite.InheritanceUserId,
                TemplateUsername = invite.InheritanceUser?.Username,
                Link = invite.Link,
                LinkId = invite.Id,
                Ttl = invite.Ttl,
                UserId = invite.UserId,
                Permissions = invite.Permissions?.Select(x => x.ToPermissionDto()).ToList()!,
                Variables = invite.Variables?.ToDictionary(x => x.Name, x => x.Value)!,
            };
        }
    }
}
