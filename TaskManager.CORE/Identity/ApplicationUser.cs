using Microsoft.AspNetCore.Identity;

namespace TaskManager.Core.Identity
{
    public  class ApplicationUser:IdentityUser<Guid>
    {
         public string? PersonName {  get; set; }
        public string? FirstName { get; set; }
        public DateTime? DateOfBirth { get; set; }


    }
}
