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
    
    public async Task<PositionDetails?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Positions
            .Where(p => p.Id == id)
            .Select(p => new PositionDetails(
                p.Id,
                p.Title,
                p.ShortDescription,
                p.MaximumProjects,
                p.Attributes
                    .OrderBy(a => a.DisplayOrder)
                    .Select(a => new PositionAttributeDetails(
                        a.AttributeId,
                        a.IsRequired,
                        a.DisplayOrder))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<PositionResultStatus> UpdateAsync(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MaximumProjects < 0)
        {
            return PositionResultStatus.InvalidMaximumProjects;
        }

        var attributeIds = request.Attributes
            .Select(a => a.AttributeId)
            .ToList();

        if (attributeIds.Count != attributeIds.Distinct().Count())
        {
            return PositionResultStatus.DuplicateAttribute;
        }

        var validCount = await _context.Attributes
            .CountAsync(
                a => attributeIds.Contains(a.Id),
                cancellationToken);

        if (validCount != attributeIds.Count)
        {
            return PositionResultStatus.InvalidAttribute;
        }

        var position = await _context.Positions
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (position is null)
        {
            return PositionResultStatus.NotFound;
        }

        position.Title = request.Title.Trim();
        position.ShortDescription = request.ShortDescription.Trim();
        position.MaximumProjects = request.MaximumProjects;
        position.UpdatedAt = DateTime.UtcNow;

        _context.PositionAttributes.RemoveRange(
            position.Attributes);

        position.Attributes.Clear();

        foreach (var item in request.Attributes)
        {
            position.Attributes.Add(new PositionAttribute
            {
                AttributeId = item.AttributeId,
                IsRequired = item.IsRequired,
                DisplayOrder = item.DisplayOrder
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return PositionResultStatus.Success;
    }
    
    public async Task<PositionResultStatus> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (position is null)
        {
            return PositionResultStatus.NotFound;
        }

        _context.Positions.Remove(position);

        await _context.SaveChangesAsync(cancellationToken);

        return PositionResultStatus.Success;
    }
    
    
    public async Task<PositionResultStatus> DuplicateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await _context.Positions
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (source is null)
        {
            return PositionResultStatus.NotFound;
        }

        var copy = new Position
        {
            Title = $"{source.Title} Copy",
            ShortDescription = source.ShortDescription,
            MaximumProjects = source.MaximumProjects
        };

        foreach (var item in source.Attributes)
        {
            copy.Attributes.Add(new PositionAttribute
            {
                AttributeId = item.AttributeId,
                IsRequired = item.IsRequired,
                DisplayOrder = item.DisplayOrder
            });
        }

        _context.Positions.Add(copy);

        await _context.SaveChangesAsync(cancellationToken);

        return PositionResultStatus.Success;
    }
}