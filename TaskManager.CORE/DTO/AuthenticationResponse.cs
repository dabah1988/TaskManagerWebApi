using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTO
{
    public  class AuthenticationResponse
    {
        public string PersonName { get; set; }= string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime  ExpirationTime { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpirationDatetime { get; set; } 
    }
}
