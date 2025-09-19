using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTO
{
    public  class LoginDTO
    {
        [Required(ErrorMessage ="Email cannot be null")]
        [EmailAddress(ErrorMessage ="Email is not in proper format")]
        public string Email { get;set; } = string.Empty;
        [Required(ErrorMessage ="email is required ")]
        public string Password { get;set; } = string.Empty;
    }
}
