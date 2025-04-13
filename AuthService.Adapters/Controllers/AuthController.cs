using AuthService.Adapters.Extensions;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var user = await authDb.GetUserForAuth(request.Login, true);

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

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest request)
        {
            if (request.Password != request.PasswordConfirm)
                return BadRequest();

            request.Password = passwordHasher.HashPassword(null!, request.Password);

            var inviteExisted = await authDb.InviteExisted(request.Invite);
            if (!inviteExisted)
            {
                return BadRequest();
            }

            var registered = await authDb.RegisterUser(request);
            
            if (registered == null)
            {
                return BadRequest();
            }

            registered.User.Password = null;
            registered.AccessToken = registered.User.GenerateJwtToken(config.JwtSettings);

            return Ok(registered);
        }
    }
}
