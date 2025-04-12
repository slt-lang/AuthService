using AuthService.Adapters.Database;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Adapters.Controllers
{
    [ApiController]
    [Route("/users/")]
    public class UsersController(IAuthDb authDb, Config config, IPasswordHasher<string> passwordHasher) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CreateUserResult>> Create([FromBody] UserDto request)
        {
            request.Password = passwordHasher.HashPassword(null!, request.Password!);
            var result = await authDb.CreateUser(request);
            if (result == CreateUserResult.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("get/{userId}")]
        public async Task<ActionResult<UserDto>> Get(int userId)
        {
            var result = await authDb.GetUser(userId);
            if (result != null)
                return Ok(result);
            return NotFound();
        }
    }
}
