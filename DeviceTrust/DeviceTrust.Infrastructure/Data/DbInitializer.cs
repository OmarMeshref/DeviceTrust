using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DeviceTrust.Infrastructure.Data;

public static class DbInitializer
{
    private static readonly string[] Roles = { "Admin", "Owner", "Technician" };

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        // 1. Ensure all roles exist
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Created role: {Role}", role);
            }
        }

        // 2. Ensure Admin account exists
        var adminEmail = config["SeedAdmin:Email"];
        var adminPassword = config["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("SeedAdmin:Email / SeedAdmin:Password not set in user-secrets — skipping admin seed.");
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is not null)
            return; // already seeded, nothing to do

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            logger.LogInformation("Seeded Admin account: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to seed Admin: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}