using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Adapters.Controllers
{
    [ApiController]
    [Route("/invites/")]
    public class InvitesController(IAuthDb authDb, Config config) : ControllerBase
    {
        
    }
}
