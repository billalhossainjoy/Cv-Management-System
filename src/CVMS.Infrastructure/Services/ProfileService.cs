using CVMS.Application.Profiles;
using CVMS.Application.Services;
using CVMS.Domain.Entities;
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
}