using CVMS.Application.Constants.Authorization;
using CVMS.Application.Positions;
using CVMS.Application.Services.Interfaces;
using CVMS.Web.Models.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

[Authorize(Policy = Policies.RecruiterAccess)]
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
    [AllowAnonymous]
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
    
    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id,
        CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(
            id,
            cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        var selectedAttributes = position.Attributes
            .ToDictionary(
                x => x.AttributeId,
                x => x);

        var model = new EditPositionViewModel
        {
            Id = position.Id,
            Title = position.Title,
            ShortDescription = position.ShortDescription,
            MaximumProjects = position.MaximumProjects,

            Attributes = attributes
                .Select(a =>
                {
                    var selected =
                        selectedAttributes.TryGetValue(
                            a.Id,
                            out var positionAttribute);

                    return new PositionAttributeSelectionViewModel
                    {
                        AttributeId = a.Id,
                        Name = a.Name,
                        Category = a.Category,

                        Selected = selected,

                        IsRequired =
                            selected &&
                            positionAttribute!.IsRequired
                    };
                })
                .ToList()
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EditPositionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await ReloadEditAttributesAsync(
                model,
                cancellationToken);

            return View(model);
        }

        var selectedAttributes = model.Attributes
            .Where(x => x.Selected)
            .Select((x, index) =>
                new PositionAttributeRequest(
                    x.AttributeId,
                    x.IsRequired,
                    (index + 1) * 10))
            .ToList();

        var request = new UpdatePositionRequest(
            model.Title,
            model.ShortDescription,
            model.MaximumProjects,
            selectedAttributes);

        var result = await _positionService.UpdateAsync(
            model.Id,
            request,
            cancellationToken);

        switch (result)
        {
            case PositionResultStatus.Success:
                TempData["Success"] =
                    "Position updated successfully.";

                return RedirectToAction(nameof(Index));

            case PositionResultStatus.NotFound:
                return NotFound();

            case PositionResultStatus.InvalidMaximumProjects:
                ModelState.AddModelError(
                    nameof(model.MaximumProjects),
                    "Maximum projects must be zero or greater.");
                break;

            case PositionResultStatus.InvalidAttribute:
            case PositionResultStatus.DuplicateAttribute:
                ModelState.AddModelError(
                    string.Empty,
                    "One or more selected attributes are invalid.");
                break;
        }

        await ReloadEditAttributesAsync(
            model,
            cancellationToken);

        return View(model);
    }
    
    private async Task ReloadEditAttributesAsync(
        EditPositionViewModel model,
        CancellationToken cancellationToken)
    {
        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        var submitted = model.Attributes
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
                submitted.TryGetValue(
                    a.Id,
                    out var existing);

                return new PositionAttributeSelectionViewModel
                {
                    AttributeId = a.Id,
                    Name = a.Name,
                    Category = a.Category,

                    Selected =
                        existing?.Selected ?? false,

                    IsRequired =
                        existing?.IsRequired ?? false
                };
            })
            .ToList();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionService.DeleteAsync(
            id,
            cancellationToken);

        switch (result)
        {
            case PositionResultStatus.Success:
                TempData["Success"] = "Position deleted.";
                return RedirectToAction(nameof(Index));

            case PositionResultStatus.NotFound:
                return NotFound();

            default:
                return BadRequest();
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duplicate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionService.DuplicateAsync(
            id,
            cancellationToken);

        switch (result)
        {
            case PositionResultStatus.Success:
                TempData["Success"] = "Position duplicated.";
                return RedirectToAction(nameof(Index));

            case PositionResultStatus.NotFound:
                return NotFound();

            default:
                return BadRequest();
        }
    }
}