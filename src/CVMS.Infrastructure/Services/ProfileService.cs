using CVMS.Application.Profiles;
using CVMS.Application.Services;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Entities;
using CVMS.Domain.Entities.Profiles;
using CVMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CVMS.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _context;
    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<Profile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Profiles
            .Include(p => p.Values)
            .ThenInclude(v => v.Attribute)
            .FirstOrDefaultAsync(p => p.UserId == userId,cancellationToken);
    }

    public async Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles.Include(p => p.Values)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        
        if (profile is null)
        {
            return new UpdateProfileResult(
                UpdateProfileStatus.NotFound);
        }

        _context.Entry(profile).Property(p => p.Version).OriginalValue = request.Version;
        foreach (var pv in request.Values)
        {
            var profileValue = profile.Values.SingleOrDefault(v => v.AttributeId == pv.AttributeId);
            if (profileValue is null)
            {
                return new UpdateProfileResult(UpdateProfileStatus.InvalidAttribute);
            }

            profileValue.Value = pv.Value;
        }

        profile.UpdatedAt = DateTime.UtcNow;
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new UpdateProfileResult(
                UpdateProfileStatus.Conflict);
        }

        return new UpdateProfileResult(UpdateProfileStatus.Success, profile.Version);
    }

    public async Task<bool> AddAttributeAsync(Guid userId, Guid attributeId, CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles
            .Include(p => p.Values)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (profile is null)
            return false;
        

        var attributeExists = await _context.Attributes.AnyAsync(
            a => a.Id == attributeId, cancellationToken);

        if (!attributeExists)
            return false;
        

        var alreadyAdded = profile.Values.Any(v => v.AttributeId == attributeId);
        if (alreadyAdded)
            return false;

        var profileValue = new ProfileValue
        {
            ProfileId = profile.Id,
            AttributeId = attributeId,
            Value = null
        };
        _context.ProfileValues.Add(profileValue);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<CvAttribute>> GetAvailableAttributesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _context.Profiles
            .Include(p => p.Values)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return [];
        }

        var existingAttributeIds = profile.Values.Select(v => v.AttributeId).ToList();

        return await _context.Attributes.Where(a => !existingAttributeIds.Contains(a.Id))
            .OrderBy(a => a.Category)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RemoveAttributeAsync(Guid  userId, Guid attributeId, CancellationToken cancellationToken)
    {
        var profileValue = await _context.ProfileValues
            .Include(pv => pv.Attribute)
            .FirstOrDefaultAsync(
                pv => pv.Profile.UserId == userId &&
                      pv.AttributeId == attributeId, cancellationToken);

        if (profileValue is null)
            return false;

        if (profileValue.Attribute.IsBuiltIn)
            return false;

        _context.ProfileValues.Remove(profileValue);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AttributeLookupResult>>
        SearchAvailableAttributesAsync(
            Guid userId,
            string prefix,
            CancellationToken cancellationToken)
    {
        prefix = prefix.Trim();
        Console.WriteLine(prefix);

        if (prefix.Length < 2)
        {
            return [];
        }

        return await _context.Attributes
            .Where(a =>
                EF.Functions.ILike(a.Name, $"{prefix}%") &&
                !a.ProfileValues.Any(pv =>
                    pv.Profile.UserId == userId))
            .OrderBy(a => a.Name)
            .Take(10)
            .Select(a => new AttributeLookupResult(
                a.Id,
                a.Name,
                a.Category.ToString()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AttributeLookupResult>> GetRecentlyUsedAttributesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.ProfileValues
            .Where(pv => !pv.Attribute.ProfileValues.Any(existing => existing.Profile.UserId == userId))
            .OrderByDescending(pv => pv.CreatedAt)
            .Select(pv => new AttributeLookupResult(
                pv.AttributeId,
                pv.Attribute.Name,
                pv.Attribute.Category.ToString()))
            .Distinct()
            .Take(5)
            .ToListAsync(cancellationToken);
    }
}