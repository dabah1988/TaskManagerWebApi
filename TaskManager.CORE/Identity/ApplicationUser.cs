using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Identity
{
    public  class ApplicationUser:IdentityUser<Guid>
    {
         public string? PersonName {  get; set; }
        public string? FirstName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(5000)]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDatetime { get; set; }
        public bool IsAdmin { get; set; } = true;
    }
}
