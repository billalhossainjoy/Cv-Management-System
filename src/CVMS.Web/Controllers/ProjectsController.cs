using System.Security.Claims;
using CVMS.Application.Projects;
using CVMS.Application.Services;
using CVMS.Web.Models.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var projects = await _projectService.GetProjectsAsync(
            userId.Value,
            cancellationToken);

        var model = projects
            .Select(p => new ProjectListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateProjectViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateProjectViewModel model,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        if (!ModelState.IsValid)
            return View(model);

        var request = new CreateProjectRequest(
            model.Name,
            model.StartDate,
            model.EndDate,
            model.Description);

        var result = await _projectService.CreateProjectAsync(
            userId.Value,
            request,
            cancellationToken);

        switch (result)
        {
            case ProjectResultStatus.Success:
                return RedirectToAction(nameof(Index));

            case ProjectResultStatus.InvalidDateRange:
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "End date cannot be before start date.");

                return View(model);

            case ProjectResultStatus.Failure:
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create project.");

                return View(model);

            default:
                return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        Console.WriteLine($"PROJECT ID: {id}");
        Console.WriteLine($"USER ID: {userId}");

        var project = await _projectService.GetProjectAsync(
            userId.Value,
            id,
            cancellationToken);

        Console.WriteLine($"PROJECT FOUND: {project is not null}");

        if (project is null)
            return NotFound();

        var model = new EditProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Description = project.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EditProjectViewModel model,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        if (!ModelState.IsValid)
            return View(model);

        var request = new UpdateProjectRequest(
            model.Name,
            model.StartDate,
            model.EndDate,
            model.Description);

        var result = await _projectService.UpdateProjectAsync(
            userId.Value,
            model.Id,
            request,
            cancellationToken);

        switch (result)
        {
            case ProjectResultStatus.Success:
                return RedirectToAction(nameof(Index));

            case ProjectResultStatus.NotFound:
                return NotFound();

            case ProjectResultStatus.InvalidDateRange:
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "End date cannot be before start date.");

                return View(model);

            case ProjectResultStatus.Failure:
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update project.");

                return View(model);

            default:
                return BadRequest();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized();

        var deleted = await _projectService.DeleteProjectAsync(
            userId.Value,
            id,
            cancellationToken);

        switch (deleted)
        {
            case ProjectResultStatus.Success:
                return RedirectToAction(nameof(Index));

            case ProjectResultStatus.NotFound:
                return NotFound();

            case ProjectResultStatus.Failure:
                TempData["Error"] = "Unable to delete project.";
                return RedirectToAction(nameof(Index));

            default:
                return BadRequest();
        }
    }
}
