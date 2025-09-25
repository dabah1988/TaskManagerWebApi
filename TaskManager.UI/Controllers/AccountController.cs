using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;
using TaskManager.Core.ServicesContract;
using Utilitaire;

namespace TaskManager.UI.Controllers
{
    /// <summary>
    /// For user auhentication 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtService _jwtService;
        /// <summary>
        /// For user registration and login
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
          RoleManager<ApplicationRole> roleManager,
          ILogger<AccountController> logger,
          IJwtService jwtService
          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                String.Join("|", ModelState.Values.SelectMany(v => v.Errors).ToList().Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }
            bool isEmailExist = await IsEmailAlreadyRegister(registerDTO.Email);
            if (isEmailExist) return Problem("Email is already used");
            var user = new ApplicationUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                string roleName = string.Empty;
                if (user.Email == ConstantValues.emailAdmin)
                {
                    roleName = "Admin";
                    user.IsAdmin = true;
                }
                else
                {
                    roleName = "User";
                    user.IsAdmin = false;
                }
                await CreateRoleAsync(_roleManager, roleName);

                // 📌 Affecte le rôle à l’utilisateur
                await _userManager.AddToRoleAsync(user, roleName);

          

                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user );
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDatetime = authenticationResponse.RefreshTokenExpirationDatetime;
                await _userManager.UpdateAsync(user);

                // 📌 Vérifie/crée le rôle avant de l’assigner
              

                _logger.LogInformation("User created a new account with password.");
                return Ok(authenticationResponse);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        private async Task<bool> IsEmailAlreadyRegister(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return (user != null);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                string messageError = string.Join("|",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { Message = messageError });
            }
            // Vérifier si l’utilisateur existe
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur inexistant: {Email}", loginDTO.Email);
                return Unauthorized(new { Message = "Email ou mot de passe incorrect" });
            }

            // Tentative de connexion
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, // ⚠️ bien utiliser UserName ici
                loginDTO.Password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                _logger.LogInformation("Connexion réussie pour {Email}", loginDTO.Email);
                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);
             
               
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDatetime = authenticationResponse.RefreshTokenExpirationDatetime;
                await _userManager.UpdateAsync(user);
                return Ok(authenticationResponse);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Compte verrouillé: {Email}", loginDTO.Email);
                return Unauthorized(new { Message = "Compte verrouillé. Réessayez plus tard." });
            }

            _logger.LogWarning("Tentative de connexion invalide pour {Email}", loginDTO.Email);
            return Unauthorized(new { Message = "Email ou mot de passe incorrect" });
        }

        /// <summary>
        /// Logout User
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateAccessToken(TokenModel tokenModel)
        {
             if(tokenModel==null || string.IsNullOrEmpty(tokenModel.Token) || string.IsNullOrEmpty(tokenModel.RefreshToken))
            {
           return BadRequest("Invalid client request");
            }
             string jwtToken = tokenModel.Token;
           ClaimsPrincipal?  principals =   _jwtService.GetPrincipalFromJwtToken(jwtToken);
            if(principals is null ) return BadRequest("Invalid Jwt acess Token");
            string userEmail = principals.FindFirstValue(ClaimTypes.Email)?? string.Empty; 
            ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail);
            if(user is null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpirationDatetime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token");
            }
         AuthenticationResponse authenticationResponse =    _jwtService.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDatetime = authenticationResponse.RefreshTokenExpirationDatetime;
            await _userManager.UpdateAsync(user);
            return Ok(authenticationResponse);
        }

        private async Task<ApplicationRole?> CreateRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            // Vérifie si le rôle existe déjà
            var role = await roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                role = new ApplicationRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    // Gérer les erreurs si besoin
                    throw new Exception($"Échec lors de la création du rôle {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            return role;
        }


    }
}
