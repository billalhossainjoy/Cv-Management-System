using System.Security.Claims;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Attributes;
using CVMS.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.ViewComponents.Account;

public class AccountViewComponent : ViewComponent
{
    private readonly IProfileService _profileService;

    public AccountViewComponent(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var httpUser = HttpContext.User;

        if (httpUser.Identity?.IsAuthenticated != true)
        {
            return View("Default", new AccountMenuViewModel
            {
                IsAuthenticated = false
            });
        }

        var displayName = httpUser.Identity.Name ?? "Account";

        var userIdValue =
            httpUser.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdValue, out var userId))
        {
            var profile = await _profileService.GetProfileAsync(
                userId,
                HttpContext.RequestAborted);

            if (profile is not null)
            {
                var firstName = profile.Values
                    .FirstOrDefault(x =>
                        x.AttributeId == BuiltInAttributes.FirstName)
                    ?.Value;

                var lastName = profile.Values
                    .FirstOrDefault(x =>
                        x.AttributeId == BuiltInAttributes.LastName)
                    ?.Value;

                var fullName =
                    $"{firstName} {lastName}".Trim();

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    displayName = fullName;
                }
            }
        }

        return View("Default", new AccountMenuViewModel
        {
            IsAuthenticated = true,
            DisplayName = displayName
        });
    }
}