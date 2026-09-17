using CVMS.Application.Constants.Authorization;
using CVMS.Infrastructure.Identity;
using CVMS.Web.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AppUserManager = Microsoft.AspNetCore.Identity.UserManager<CVMS.Infrastructure.Identity.ApplicationUser>;
using AppSignInManager = Microsoft.AspNetCore.Identity.SignInManager<CVMS.Infrastructure.Identity.ApplicationUser>;

namespace CVMS.Web.Controllers;

[Authorize]
public sealed class AccountController : Controller
{
    private readonly AppUserManager _userManager;
    private readonly AppSignInManager _signInManager;

    public AccountController(AppUserManager userManager,AppSignInManager  signInManager )
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName =  model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _userManager.AddToRoleAsync(user, Roles.Candidate);
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (user.IsBlocked || user.DeletedAt is not null)
        {
            ModelState.AddModelError(string.Empty, "this account is unavailable");
            return View(model);
        }

        var result =
            await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
            return View(model);
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}