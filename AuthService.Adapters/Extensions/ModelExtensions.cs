using AuthService.Adapters.Database.Models;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;

namespace AuthService.Adapters.Extensions
{
    public static class ModelExtensions
    {
        public static T ToConcretePermission<T>(this AbstractPermission permission) where T : AbstractPermission, new()
        {
            return new T()
            {
                AllowInheritance = permission.AllowInheritance,
                CreateDate = permission.CreateDate,
                EndDate = permission.EndDate,
                Id = permission.Id,
                PermissionId = permission.PermissionId,
                RefPermission = permission.RefPermission,
            };
        }

        public static T ToConcretePermission<T>(this PermissionDto permission, Action<T> addition = null!) where T : AbstractPermission, new()
        {
            var ret = new T()
            {
                CreateDate = permission.CreateDate.ToUniversalTime(),
                EndDate = permission.EndDate?.ToUniversalTime(),
                AllowInheritance = permission.AllowInheritance,
                PermissionId = permission.Permission,
            };
            addition?.Invoke(ret);
            return ret;
        }

        public static T ToConcreteVariable<T>(this AbstractVariable variable) where T : AbstractVariable, new()
        {
            return new T()
            {
                Id = variable.Id,
                Name = variable.Name,
                RefVariable = variable.RefVariable,
                Value = variable.Value,
            };
        }

        public static T ToConcreteVariable<T>(this KeyValuePair<Variable, string> variable, Action<T> addition = null!) where T : AbstractVariable, new()
        {
            var ret = new T()
            {
                Name = variable.Key,
                Value = variable.Value,
            };
            addition?.Invoke(ret);
            return ret;
        }
    }
}
