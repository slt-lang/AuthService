using AuthService.Adapters.Database.Models;
using AuthService.Domain;
using AuthService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Adapters.Database
{
    public class AuthDb(IDateTime dateTime, IMemoryCache memoryCache, AuthServiceContext db, Config config) : IAuthDb
    {

    }
}
