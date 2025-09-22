using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;

namespace TaskManager.Core.ServicesContract
{
    public  interface IJwtService
    {
       AuthenticationResponse  CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
