using AuthService.Domain;
using Microsoft.IdentityModel.Tokens;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using sltlang.Common.AuthService.Models;
using sltlang.Common.Common.Extensions;
using System.ComponentModel;
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
                if (!permission.Value.EndDate.HasValue || permission.Value.EndDate.Value < DateTime.Now)
                    claims.Add(new Claim("Permission", permission.Key.ToString()));
            }

            foreach (var variable in EnumExtensions.EnumHelper<Variable>.Values)
            {
                if (user.Variables.ContainsKey(variable))
                {
                    claims.Add(new Claim("Variable+" + variable.ToString(), user.Variables[variable].SerializeByEnum(variable)));
                }
                else if (variable.HasAttribute<Variable, AlwaysTranfsferAttribute>())
                {
                    var value = variable.HasAttribute<Variable, DefaultValueAttribute>()
                        ? EnumExtensions.EnumHelper<Variable>.AttributeHelper<DefaultValueAttribute>.With(variable, x => x!.Value)
                        : null;
                    claims.Add(new Claim("Variable+" + variable.ToString(), value?.SerializeByEnum(variable) ?? "null"));
                }
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
