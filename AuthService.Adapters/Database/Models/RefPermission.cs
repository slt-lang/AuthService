using sltlang.Common.AuthService.Enums;

namespace AuthService.Adapters.Database.Models
{
    public class RefPermission
    {
        public Permission Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
