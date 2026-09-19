using Microsoft.AspNetCore.Identity;

namespace MyProject.Controllers
{
    public static class IdentitySeeder
    {

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Servant" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
