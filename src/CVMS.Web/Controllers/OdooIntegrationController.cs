using CVMS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

[ApiController]
[Route("api/odoo-integration")]
[AllowAnonymous]
public class OdooIntegrationController : ControllerBase
{
    private readonly IPositionService _positionService;

    public OdooIntegrationController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet("position-stats")]
    public async Task<IActionResult> GetPositionStats([FromQuery] string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token)) 
        {
            return BadRequest(new { Error = "API Token is required." });
        }

        var position = await _positionService.GetByTokenAsync(token, cancellationToken);
        if (position == null) 
        {
            return Unauthorized(new { Error = "Invalid API Token." });
        }

        var response = new
        {
            PositionTitle = position.Title,
            Attributes = position.Attributes.Select(a => new 
            {
                Title = a.AttributeId.ToString(),
                Type = "number",
                AggregatedValue = "Avg: 0, Min: 0, Max: 0"
            }).ToArray()
        };

        return Ok(response);
    }
}

