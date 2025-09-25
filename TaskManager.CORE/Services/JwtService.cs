using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;
using TaskManager.Core.ServicesContract;
using Utilitaire;

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
            Guid miJti = Guid.NewGuid();
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.Id.ToString()), //Subject of the token
              new Claim(JwtRegisteredClaimNames.Jti , (miJti).ToString() ), //Unique identifier for the token  
               new Claim(JwtRegisteredClaimNames.Iat,        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),       ClaimValueTypes.Integer64),
              new Claim(JwtRegisteredClaimNames.NameId , user.Email ), //Unique name identifier for user
              new Claim(JwtRegisteredClaimNames.Name , user.PersonName ), //name of user
              new Claim(JwtRegisteredClaimNames.Email , user.Email ), //name of user
            };

            // 🔹 Ajouter le rôle Admin comme claim
            if (user.IsAdmin)
                claims = claims.Concat(new[] { new Claim(ClaimTypes.Role, ConstantValues.roleAdmin) }).ToArray();
            else
                claims = claims.Concat(new[] { new Claim(ClaimTypes.Role, ConstantValues.roleUser) }).ToArray();


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


            return new AuthenticationResponse()
            {
                Token = token,
                ExpirationTime = expiration,
                Email = user.Email,
                PersonName = user.PersonName ?? string.Empty,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDatetime = 
                DateTime.UtcNow.AddMinutes( Convert.ToDouble(_configuration["refreshToken:Expiration_minutes"])) // Refresh token valid for 7 days
            };
        }
        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principals = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principals;
        }
        //public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        //{
        //    throw new NotImplementedException();
        //}

        private string GenerateRefreshToken()
        {
            byte[]  bytes = new byte[64];
            var randonNumber = RandomNumberGenerator.Create();   
            randonNumber.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

    }
}
