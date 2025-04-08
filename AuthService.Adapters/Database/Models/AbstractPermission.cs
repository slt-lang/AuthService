using sltlang.Common.AuthService.Enums;

namespace AuthService.Adapters.Database.Models
{
    public class AbstractPermission
    {
        public int Id { get; set; }
        public Permission PermissionId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AllowInheritance { get; set; }
    }
}
