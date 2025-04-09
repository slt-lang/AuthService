using AuthService.Domain;
using Microsoft.IdentityModel.Tokens;
using sltlang.Common.AuthService.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Adapters.Extensions
{
    public static class JwtExtensions
    {
        public static string GenerateJwtToken(this UserDto user, JwtSettings settings)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("Permission", permission.Key.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationMinutes = settings.ExpirationMinutes;
            var expiry = DateTime.Now.AddMinutes(expirationMinutes);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
