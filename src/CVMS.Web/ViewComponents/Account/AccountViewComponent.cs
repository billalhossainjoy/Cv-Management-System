using System.Security.Claims;
using CVMS.Application.Services;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CVMS.Infrastructure.Identity;
using CVMS.Infrastructure.Persistence;

namespace CVMS.Web.ViewComponents.Account;

public class AccountViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IProfileService _profileService;

    public AccountViewComponent(
        UserManager<ApplicationUser> userManager
        , ApplicationDbContext context
        , IProfileService profileService)
    {
        _userManager = userManager;
        _context = context;
        _profileService = profileService;
        
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
        var httpUser = HttpContext.User;
        if (httpUser.Identity?.IsAuthenticated != true)
        {
            return View<ApplicationUser?>("Default", null);
        }

        var userIdValue = httpUser.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return View<ApplicationUser?>("Default", null);
        }

        var profile = await _profileService.GetProfileAsync(
            userId,
            HttpContext.RequestAborted);

        if (profile == null)
        {
            return View<ApplicationUser?>("Default", null);
        }

        var firstName = profile.Values.FirstOrDefault(v => v.AttributeId == BuiltInAttributes.FirstName)?.Value;
        var lastName = profile.Values.FirstOrDefault(v => v.AttributeId == BuiltInAttributes.LastName)?.Value;
        var fullName = $"{firstName} {lastName}";

        
        return View("Default", fullName);
    }
}