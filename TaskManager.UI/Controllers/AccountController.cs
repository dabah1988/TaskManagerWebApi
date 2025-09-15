using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTO;
using TaskManager.Core.Identity;

namespace TaskManager.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController:ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
          RoleManager<ApplicationRole> _roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = _roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
             String.Join("|",   ModelState.Values.SelectMany(v => v.Errors).ToList().Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser
            {
                UserName = registerDTO.Username,
                Email = registerDTO.Email,
                PersonName = registerDTO.FullName,
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
        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegister( string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Ok(new { isRegistered = true, Message = "Email is already used" });
            }
            return Ok(new { isRegistered = false });
        }   


    }
}
