using AuthenticationService.Data.Context;
using AuthenticationService.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationService.Data.Repositories;

public static class SeedManager
{
    public static async Task Seed(IServiceProvider services)
    {
        await SeedRoles(services);
        await SeedAdminUser(services);
    }

    private static async Task SeedRoles(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await roleManager.CreateAsync(new IdentityRole(Role.Admin));
        await roleManager.CreateAsync(new IdentityRole(Role.User));
    }

    private static async Task SeedAdminUser(IServiceProvider services)
    {
        var context = services.GetRequiredService<AuthDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var adminUser = await context.Users.FirstOrDefaultAsync(user => user.UserName == "AuthenticationAdmin");

        if (adminUser is null)
        {
            adminUser = new User { UserName = "AuthenticationAdmin", Email = "adunseayorinde@gmail.com" };
            await userManager.CreateAsync(adminUser, Guid.NewGuid().ToString());
            await userManager.AddToRoleAsync(adminUser, Role.Admin);
        }

    }
   
}