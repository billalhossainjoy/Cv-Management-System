using CVMS.Application.Attribute;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Entities;
using CVMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVMS.Infrastructure.Services;

public class AttributeService:IAttributeService
{
    private readonly ApplicationDbContext _context;
    public AttributeService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyList<AttributeListItem>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Attributes
            .OrderBy(a => a.Category)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .Select(a => new AttributeListItem(
                a.Id,
                a.Name,
                a.Description,
                a.Category.ToString(),
                a.Type.ToString(),
                a.IsBuiltIn))
            .ToListAsync(cancellationToken);
    }   
    
    public async Task<AttributeDetails?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Attributes
            .Where(a => a.Id == id)
            .Select(a => new AttributeDetails(
                a.Id,
                a.Name,
                a.Description,
                a.Category,
                a.Type,
                a.IsBuiltIn))
            .FirstOrDefaultAsync(cancellationToken);
    }
    

    public async Task<AttributeResultStatus> CreateAsync(
        CreateAttributeRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var duplicateExists = await _context.Attributes
            .AnyAsync(
                a => EF.Functions.ILike(a.Name, name),
                cancellationToken);

        if (duplicateExists)
        {
            return AttributeResultStatus.DuplicateName;
        }

        var attribute = new CvAttribute(
            name,
            request.Description,
            request.Category,
            request.Type);

        _context.Attributes.Add(attribute);

        await _context.SaveChangesAsync(cancellationToken);

        return AttributeResultStatus.Success;
    }

    public async Task<AttributeResultStatus> UpdateAsync(
        Guid id,
        UpdateAttributeRequest request,
        CancellationToken cancellationToken)
    {
        var attribute = await _context.Attributes
            .FirstOrDefaultAsync(
                a => a.Id == id,
                cancellationToken);

        if (attribute is null)
        {
            return AttributeResultStatus.NotFound;
        }

        var name = request.Name.Trim();

        var duplicateExists = await _context.Attributes
            .AnyAsync(
                a =>
                    a.Id != id &&
                    EF.Functions.ILike(a.Name, name),
                cancellationToken);

        if (duplicateExists)
        {
            return AttributeResultStatus.DuplicateName;
        }

        attribute.Update(
            name,
            request.Description,
            request.Category,
            request.Type);

        attribute.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return AttributeResultStatus.Success;
    }

    public async Task<AttributeResultStatus> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attribute = await _context.Attributes
            .FirstOrDefaultAsync(
                a => a.Id == id,
                cancellationToken);

        if (attribute is null)
        {
            return AttributeResultStatus.NotFound;
        }

        if (attribute.IsBuiltIn)
        {
            return AttributeResultStatus.BuiltInProtected;
        }

        _context.Attributes.Remove(attribute);

        await _context.SaveChangesAsync(cancellationToken);

        return AttributeResultStatus.Success;
    }
}