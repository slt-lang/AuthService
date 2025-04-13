using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;
using sltlang.Common.AuthService.Contracts;
using sltlang.Common.AuthService.Dto;

namespace AuthService.Adapters.Controllers
{
    [ApiController]
    [Route("/invites/")]
    public class InvitesController(IAuthDb authDb, Config config) : ControllerBase
    {
        [HttpPost("new")]
        public async Task<ActionResult<CreateInviteLinkResponse>> Create(InviteLinkDto request)
        {
            var result = await authDb.CreateInvite(request);
            return Ok(result);
        }
    }
}
