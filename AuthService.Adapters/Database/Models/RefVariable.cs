using sltlang.Common.AuthService.Enums;

namespace AuthService.Adapters.Database.Models
{
    public class RefVariable
    {
        public Variable Id { get; set; }
        public string Type { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
