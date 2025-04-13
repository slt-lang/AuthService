using sltlang.Common.AuthService.Models;

namespace AuthService.Domain
{
    public class Config
    {
        public JwtSettings JwtSettings { get; set; } = default!;
    }
}
