using Microsoft.AspNetCore.Identity;

namespace CVMS.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        string[] roles =
        [
            "Candidate",
            "Recruiter",
            "Administrator"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));
            }
        }
        
        
        await CreateUserAsync(
            userManager,
            email: "admin@example.com",
            password: "Admin123!",
            role: "Administrator");

        await CreateUserAsync(
            userManager,
            email: "recruiter@example.com",
            password: "Recruiter123!",
            role: "Recruiter");
        
    }
    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(
            user,
            password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Could not create {role}: {errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Could not assign {role}: {errors}");
        }
    }
    
    
}