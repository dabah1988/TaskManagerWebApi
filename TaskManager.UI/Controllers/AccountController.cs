using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;

namespace TaskManager.UI.Controllers
{
    /// <summary>
    /// For user auhentication 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController:ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

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
          ILogger<AccountController> logger
          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
             String.Join("|",   ModelState.Values.SelectMany(v => v.Errors).ToList().Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }
            bool isEmailExist = await IsEmailAlreadyRegister(registerDTO.Email) ;
            if(isEmailExist) return Problem("Email is already used");
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
                // Optionally assign a default role to the user
                // await _userManager.AddToRoleAsync(user, "User");
                _logger.LogInformation("User created a new account with password.");
                return Ok(new { Message = "User registered successfully" });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState); 
        }
       
        private async Task<bool> IsEmailAlreadyRegister( string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return (user != null);
         
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
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

            //// Vérifier si l’email est confirmé
            //if (!user.EmailConfirmed)
            //{
            //    return Unauthorized(new { Message = "Email non confirmé" });
            //}

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

                return Ok(new
                {
                    PersonName = user.PersonName,
                    Login = user.Email,
                    PhoneNumber = user.PhoneNumber
                });
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
        public async Task<IActionResult> Logout ()
        {
             await _signInManager.SignOutAsync();
            return NoContent();
        }

        }
}
