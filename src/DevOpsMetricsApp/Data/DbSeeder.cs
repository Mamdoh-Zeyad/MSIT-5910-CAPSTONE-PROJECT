using Microsoft.AspNetCore.Identity;

namespace DevOpsMetricsApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Administrator", "TeamLeader", "Developer" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@devops.com");
            if (adminUser == null)
            {
                var admin = new IdentityUser { UserName = "admin@devops.com", Email = "admin@devops.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Admin@123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Administrator");
            }

            var leaderUser = await userManager.FindByEmailAsync("lead@devops.com");
            if (leaderUser == null)
            {
                var leader = new IdentityUser { UserName = "lead@devops.com", Email = "lead@devops.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(leader, "Lead@123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(leader, "TeamLeader");
            }

            var devUser = await userManager.FindByEmailAsync("dev@devops.com");
            if (devUser == null)
            {
                var dev = new IdentityUser { UserName = "dev@devops.com", Email = "dev@devops.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(dev, "Dev@123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(dev, "Developer");
            }
        }
    }
}