using CVMS.Application.Constants.Authorization;
using CVMS.Application.Services;
using CVMS.Infrastructure.Identity;
using CVMS.Infrastructure.Persistence;
using CVMS.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProjectService, ProjectService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.RecruiterAccess, policy => policy.RequireRole(Roles.Recruiter, Roles.Administrator));
            options.AddPolicy(Policies.AdministratorOnly, policy => policy.RequireRole(Roles.Administrator));
        });

        return services;
    }
}