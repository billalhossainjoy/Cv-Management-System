using CVMS.Application.Services.Interfaces;
using CVMS.Application.TechnologyTags;
using CVMS.Domain.Entities;
using CVMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVMS.Infrastructure.Services;

public class TechnologyTagService: ITechnologyTagService
{
    private readonly ApplicationDbContext _context;

    public TechnologyTagService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<TechnologyTagLookup>> SearchAsync(
        string prefix,
        CancellationToken cancellationToken)
    {
        prefix = prefix.Trim();

        if (prefix.Length < 2)
        {
            return [];
        }

        return await _context.TechnologyTags
            .Where(t =>
                EF.Functions.ILike(
                    t.Name,
                    $"{prefix}%"))
            .OrderBy(t => t.Name)
            .Take(10)
            .Select(t => new TechnologyTagLookup(
                t.Id,
                t.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<TechnologyTagLookup> GetOrCreateAsync(
        string name,
        CancellationToken cancellationToken)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Technology tag name is required.",
                nameof(name));
        }

        var existing = await _context.TechnologyTags
            .Where(t =>
                EF.Functions.ILike(
                    t.Name,
                    name))
            .Select(t => new TechnologyTagLookup(
                t.Id,
                t.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var tag = new TechnologyTag
        {
            Name = name
        };

        _context.TechnologyTags.Add(tag);

        await _context.SaveChangesAsync(cancellationToken);

        return new TechnologyTagLookup(
            tag.Id,
            tag.Name);
    }
    
    
}