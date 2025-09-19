using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;
using TaskManager.Core.ServicesContract;

namespace TaskManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
                _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
           DateTime expiration =  DateTime.UtcNow.AddMinutes( Convert.ToDouble( _configuration["Jwt:Expiration_minutes"]));
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.Id.ToString()), //Subject of the token
              new Claim(JwtRegisteredClaimNames.Jti , new Guid().ToString() ), //Unique identifier for the token
              new Claim(JwtRegisteredClaimNames.Iat , DateTime.Now.ToString() ), //DateTime of generation
              new Claim(JwtRegisteredClaimNames.NameId , user.Email ), //Unique name identifier for user
              new Claim(JwtRegisteredClaimNames.Name , user.PersonName ), //name of user
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                 _configuration["Jwt:Issuer"],
                 _configuration["Jwt:Audience"],
                 claims,
                 expires: expiration,
                 signingCredentials:signingCredentials
                );
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
         string token =   tokenHandler.WriteToken(tokenGenerator);


            return new AuthenticationResponse() { Token = token, 
                ExpirationTime = expiration, 
                Email = user.Email, 
                PersonName = user.PersonName ?? string.Empty  
            };
        }
    }
}
