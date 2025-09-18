using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="Username is required")]
        public  string? Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        [Remote(action: "IsEmailAlreadyRegister",controller:  "Account",ErrorMessage="Email is already used")]
        public  string? Email { get; set; }

        public string? login { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
