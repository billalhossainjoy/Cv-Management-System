using CVMS.Application.Positions;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Entities.Positions;
using CVMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVMS.Infrastructure.Services;

public class PositionService: IPositionService
{
    private readonly ApplicationDbContext _context;

    public PositionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PositionListItem>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Positions
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PositionListItem(
                p.Id,
                p.Title,
                p.ShortDescription,
                p.MaximumProjects,
                p.Attributes.Count))
            .ToListAsync(cancellationToken);
    }


    public async Task<PositionResultStatus> CreateAsync(
        CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MaximumProjects < 0)
        {
            return PositionResultStatus.InvalidMaximumProjects;
        }
        
        var attributeIds = request.Attributes
            .Select(x => x.AttributeId)
            .ToList();

        if (attributeIds.Count != attributeIds.Distinct().Count())
        {
            return PositionResultStatus.DuplicateAttribute;
        }
        
        
        var validAttributeCount = await _context.Attributes
            .CountAsync(
                a => attributeIds.Contains(a.Id),
                cancellationToken);
        
        if (validAttributeCount != attributeIds.Count)
        {
            return PositionResultStatus.InvalidAttribute;
        }
        
        var position = new Position
        {
            Title = request.Title.Trim(),
            ShortDescription = request.ShortDescription.Trim(),
            MaximumProjects = request.MaximumProjects
        };
        
        foreach (var item in request.Attributes)
        {
            position.Attributes.Add(new PositionAttribute
            {
                AttributeId = item.AttributeId,
                IsRequired = item.IsRequired,
                DisplayOrder = item.DisplayOrder
            });
        }
        
        _context.Positions.Add(position);

        await _context.SaveChangesAsync(cancellationToken);

        return PositionResultStatus.Success;
        
    }
}