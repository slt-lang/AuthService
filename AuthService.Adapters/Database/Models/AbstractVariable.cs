using sltlang.Common.AuthService.Enums;

namespace AuthService.Adapters.Database.Models
{
    public class AbstractVariable
    {
        public int Id { get; set; }
        public Variable Name { get; set; }
        public string? Value { get; set; }

        public virtual RefVariable RefVariable { get; set; } = default!;
    }
}
