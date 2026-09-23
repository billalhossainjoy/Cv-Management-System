using System.Diagnostics;
using CVMS.Application.Services.Interfaces;
using CVMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using CVMS.Web.Models;
using CVMS.Web.Models.Home;
using Microsoft.AspNetCore.Identity;

namespace CVMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IPositionService _positionService;
    private readonly IAttributeService _attributeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        IPositionService positionService,
        IAttributeService attributeService,
        UserManager<ApplicationUser> userManager)
    {
        _positionService = positionService;
        _attributeService = attributeService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var positions = await _positionService.GetAllAsync(
            cancellationToken);

        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        var candidates = await _userManager
            .GetUsersInRoleAsync("Candidate");

        var recruiters = await _userManager
            .GetUsersInRoleAsync("Recruiter");

        var model = new HomeViewModel
        {
            PositionCount = positions.Count,
            CandidateCount = candidates.Count,
            RecruiterCount = recruiters.Count,
            AttributeCount = attributes.Count,

            LatestPositions = positions
                .Take(5)
                .ToList()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
