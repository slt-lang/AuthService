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

namespace AuthService.Domain.Extensions
{
    public static class DtoExtensions
    {
        public static UserDto ToUserDto(this User user, bool password = false)
        {
            return new UserDto()
            {
                Enabled = user.Enabled,
                Id = user.Id,
                IsTemplate = user.IsTemplate,
                Password = password ? user.PasswordHash : null,
                Permissions = user.Permissions?.ToDictionary(x => x.PermissionId, x => x.ToPermissionDto()),
                RegistrationDate = user.RegisterDate,
                Username = user.Username,
                Variables = user.Variables?.ToDictionary(x => x.Name, x => x.Value != null ? JsonSerializer.Deserialize(x.Value, x.Name.GetVariableType()) : null),
            };
        }

        private static ConcurrentDictionary<Variable, Type> variableTypes = new ConcurrentDictionary<Variable, Type>(typeof(Variable).GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => new KeyValuePair<Variable, Type>((Variable)x.GetRawConstantValue(), x.GetCustomAttribute<ComponentTypeAttribute>().Type)));
        // todo перенести в sltlang.Common
        public static Type GetVariableType(this Variable variable)
        {
            return variableTypes[variable];
        }

        public static PermissionDto ToPermissionDto(this AbstractPermission permission)
        {
            return new PermissionDto()
            {
                CreateDate = permission.CreateDate,
                //EndDate = permission.EndDate, //todo
                Id = permission.Id,
                IsAllowInheritance = permission.AllowInheritance,
                //IsTemporary = permission.IsTemporary,
                Permission = permission.PermissionId,
            };
        }
    }
}
