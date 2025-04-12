using AuthService.Adapters.Extensions;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;
using sltlang.Common.AuthService.Enums;
using LoginRequest = sltlang.Common.AuthService.Contracts.LoginRequest;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthController(IAuthDb authDb, Config config, IPasswordHasher<string> passwordHasher) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await authDb.GetUser(request.Username, true);

            if (user == null || user.IsTemplate)
            {
                return Unauthorized("Invalid username or password.");
            }

            if (user.Password == null || passwordHasher.VerifyHashedPassword(null!, user.Password, request.Password) == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = user.GenerateJwtToken(config.JwtSettings);
            return Ok(new LoginResponse(){ AccessToken = token });
        }
    }
}
