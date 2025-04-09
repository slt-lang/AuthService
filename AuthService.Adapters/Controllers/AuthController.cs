using AuthService.Adapters.Database.Models;
using AuthService.Adapters.Extensions;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthController(IAuthDb authDb, Config config) : ControllerBase
    {
        //todo move to Contracts
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class LoginResponse
        {
            public string AccessToken { get; set; }
        }

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
                }
            };

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Verify the password
            if (user.Username != request.Username && request.Password != user.Password)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = user.GenerateJwtToken(config.JwtSettings);
            return Ok(new LoginResponse(){ AccessToken = token });
        }
    }
}
