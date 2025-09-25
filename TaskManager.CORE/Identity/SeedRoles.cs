using Microsoft.AspNetCore.Identity;

namespace TaskManager.Core.Identity
{
    public static  class SeedRoles
    {
        public static async Task InitializeAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }
        }
    }
}
