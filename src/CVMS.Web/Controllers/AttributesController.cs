using CVMS.Application.Attribute;
using CVMS.Application.Constants.Authorization;
using CVMS.Application.Services.Interfaces;
using CVMS.Web.Models.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

[Authorize(Policy = Policies.RecruiterAccess)]
public class AttributesController : Controller
{
    private readonly IAttributeService _attributeService;

    public AttributesController(
        IAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var attributes = await _attributeService.GetAllAsync(
            cancellationToken);

        return View(attributes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateAttributeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateAttributeViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new CreateAttributeRequest(
            model.Name,
            model.Description,
            model.Category,
            model.Type);

        var result = await _attributeService.CreateAsync(
            request,
            cancellationToken);

        switch (result)
        {
            case AttributeResultStatus.Success:
                TempData["Success"] =
                    "Attribute created successfully.";

                return RedirectToAction(nameof(Index));

            case AttributeResultStatus.DuplicateName:
                ModelState.AddModelError(
                    nameof(model.Name),
                    "An attribute with this name already exists.");

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
        var attribute = await _attributeService.GetByIdAsync(
            id,
            cancellationToken);

        if (attribute is null)
        {
            return NotFound();
        }

        var model = new EditAttributeViewModel
        {
            Id = attribute.Id,
            Name = attribute.Name,
            Description = attribute.Description,
            Category = attribute.Category,
            Type = attribute.Type,
            IsBuiltIn = attribute.IsBuiltIn
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EditAttributeViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new UpdateAttributeRequest(
            model.Name,
            model.Description,
            model.Category,
            model.Type);

        var result = await _attributeService.UpdateAsync(
            model.Id,
            request,
            cancellationToken);

        switch (result)
        {
            case AttributeResultStatus.Success:
                TempData["Success"] =
                    "Attribute updated successfully.";

                return RedirectToAction(nameof(Index));

            case AttributeResultStatus.NotFound:
                return NotFound();

            case AttributeResultStatus.DuplicateName:
                ModelState.AddModelError(
                    nameof(model.Name),
                    "An attribute with this name already exists.");

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
        var result = await _attributeService.DeleteAsync(
            id,
            cancellationToken);

        switch (result)
        {
            case AttributeResultStatus.Success:
                TempData["Success"] =
                    "Attribute deleted successfully.";

                return RedirectToAction(nameof(Index));

            case AttributeResultStatus.NotFound:
                return NotFound();

            case AttributeResultStatus.BuiltInProtected:
                TempData["Error"] =
                    "Built-in attributes cannot be deleted.";

                return RedirectToAction(nameof(Index));

            default:
                return BadRequest();
        }
    }
}