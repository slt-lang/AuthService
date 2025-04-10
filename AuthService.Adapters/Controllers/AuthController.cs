using AuthService.Adapters.Extensions;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using LoginRequest = sltlang.Common.AuthService.Contracts.LoginRequest;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthController(IAuthDb authDb, Config config) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserDto()
            {
                Username = "root",
                Password = "x",
                Permissions = new Dictionary<Permission, PermissionDto>()
                {
                    { Permission.RootPermission, new PermissionDto() }
                },
                Variables = new Dictionary<Variable, object>()
                {
                    { Variable.MaxLinkTTL, 60 },
                }
            };

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Verify the password
            if (user.Username != request.Username || request.Password != user.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = user.GenerateJwtToken(config.JwtSettings);
            return Ok(new LoginResponse(){ AccessToken = token });
        }
    }
}
