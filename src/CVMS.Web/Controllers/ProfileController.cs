using System.Security.Claims;
using CVMS.Application.Profiles;
using CVMS.Application.Services;
using CVMS.Domain.Attributes;
using CVMS.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return null;
        };
        return result;
    }
    
    public async Task<IActionResult> Index( CancellationToken cancellationToken)
    {
        var userId = GetUserId();   
        if (userId is null)
            return Unauthorized();
        
        var profile = await _profileService.GetProfileAsync(userId.Value, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            Id = profile.Id,
            Version = profile.Version,
            Values = profile.Values
                .OrderBy(x => x.Attribute.DisplayOrder)
                .Select(x => new ProfileValueViewModel
            {
                AttributeId = x.AttributeId,
                AttributeName = x.Attribute.Name,
                AttributeType = x.Attribute.Type.ToString(),
                Value = x.Value
            }).ToList()
        };
        
        return View(model);
    }

    public async Task<IActionResult> Save(ProfileViewModel model, CancellationToken cancellationToken)
    {
        var  userId = GetUserId();
        if (userId is null)
            return Unauthorized();
        
        if (!ModelState.IsValid)
            return View("Index", model);

        var request = new UpdateProfileRequest(
            model.Version,
            model.Values.Select(
                v => 
                    new UpdateProfileValueRequest(v.AttributeId, v.Value)
                ).ToList()
            );

        var result = await _profileService.UpdateProfileAsync(userId.Value, request, cancellationToken);
        switch (result.Status)
        {
            case UpdateProfileStatus.Success:
                TempData["Success"] = "profile Saved Successfully";
                return RedirectToAction(nameof(Index));
            
            case UpdateProfileStatus.Conflict:
                TempData["Error"] =
                    "Your profile was changed elsewhere. Please review the latest data.";
                return RedirectToAction(nameof(Index));

            case UpdateProfileStatus.InvalidAttribute:
                return BadRequest();

            case UpdateProfileStatus.NotFound:
                return NotFound();
            
            default:
                return BadRequest();
        }
    }
}