using AuthService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthController(IAuthDb articleDb) : ControllerBase
    {

    }
}
