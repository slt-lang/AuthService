using AuthService.Adapters.Database.Models;

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
    }
}
