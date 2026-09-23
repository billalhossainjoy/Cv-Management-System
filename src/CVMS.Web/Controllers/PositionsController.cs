using CVMS.Application.Constants.Authorization;
using CVMS.Application.Positions;
using CVMS.Application.Services.Interfaces;
using CVMS.Web.Models.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

// [Authorize(Policy = Policies.RecruiterAccess)]
public class PositionsController: Controller
{
    private readonly IPositionService _positionService;
    private readonly IAttributeService _attributeService;

    public PositionsController(
        IPositionService positionService,
        IAttributeService attributeService)
    {
        _positionService = positionService;
        _attributeService = attributeService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var positions = await _positionService.GetAllAsync(
            cancellationToken);

        return View(positions);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        var model = new CreatePositionViewModel()
        {
            Attributes = attributes
                .Select(a => new PositionAttributeSelectionViewModel
                {
                    AttributeId = a.Id,
                    Name = a.Name,
                    Category = a.Category
                })
                .ToList()
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreatePositionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await ReloadAttributesAsync(model, cancellationToken);
            return View(model);
        }

        var selected = model.Attributes
            .Where(a => a.Selected)
            .Select((a, index) =>
                new PositionAttributeRequest(
                    a.AttributeId,
                    a.IsRequired,
                    (index + 1) * 10))
            .ToList();

        var request = new CreatePositionRequest(
            model.Title,
            model.ShortDescription,
            model.MaximumProjects,
            selected);

        var result = await _positionService.CreateAsync(
            request,
            cancellationToken);

        switch (result)
        {
            case PositionResultStatus.Success:
                return RedirectToAction(nameof(Index));

            case PositionResultStatus.InvalidAttribute:
            case PositionResultStatus.DuplicateAttribute:
                ModelState.AddModelError(
                    string.Empty,
                    "One or more selected attributes are invalid.");
                break;

            case PositionResultStatus.InvalidMaximumProjects:
                ModelState.AddModelError(
                    nameof(model.MaximumProjects),
                    "Maximum projects must be zero or greater.");
                break;
        }

        await ReloadAttributesAsync(model, cancellationToken);

        return View(model);
    }
    
    private async Task ReloadAttributesAsync(
        CreatePositionViewModel model,
        CancellationToken cancellationToken)
    {
        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        var selected = model.Attributes
            .ToDictionary(
                x => x.AttributeId,
                x => new
                {
                    x.Selected,
                    x.IsRequired
                });

        model.Attributes = attributes
            .Select(a =>
            {
                selected.TryGetValue(a.Id, out var old);

                return new PositionAttributeSelectionViewModel
                {
                    AttributeId = a.Id,
                    Name = a.Name,
                    Category = a.Category,
                    Selected = old?.Selected ?? false,
                    IsRequired = old?.IsRequired ?? false
                };
            })
            .ToList();
    }
}