using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure;

public static class DatabaseSeeder
{
    private static readonly string[] Roles = ["Admin", "Organizer", "Player"];

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var context = services.GetRequiredService<ApplicationContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                EnsureSucceeded(result, $"Impossible de créer le rôle {role}.");
            }
        }

        var adminEmail = configuration["AdminSeed:Email"];
        var adminPassword = configuration["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            EnsureSucceeded(result, "Impossible de créer l'administrateur.");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Admin");
            EnsureSucceeded(result, "Impossible d'associer le rôle Admin.");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"{message} {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }
}
