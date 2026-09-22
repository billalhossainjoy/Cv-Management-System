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
        {
            return Unauthorized();
        }

        var projects = await _projectService.GetProjectsAsync(
            userId.Value,
            cancellationToken);

        var model = projects
            .Select(p => new ProjectListItemViewModel()
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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectViewModel model,  CancellationToken cancellationToken)
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
        
        var project = await _projectService.CreateProjectAsync(userId.Value, request, cancellationToken);
        
        if (!project)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to create project.");

            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var project = await _projectService.GetProjectAsync(
            userId.Value,
            id,
            cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var request = new UpdateProjectRequest(
            model.Name,
            model.StartDate,
            model.EndDate,
            model.Description);

        var updated = await _projectService.UpdateProjectAsync(
            userId.Value,
            model.Id,
            request,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _projectService.DeleteProjectAsync(
            userId.Value,
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

}