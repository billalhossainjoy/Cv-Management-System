using CVMS.Application.Profiles;
using CVMS.Domain.Entities;
using CVMS.Domain.Entities.Profiles;

namespace CVMS.Application.Services.Interfaces;

public interface IProfileService
{
    Task<Profile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task<bool> AddAttributeAsync(
        Guid userId,
        Guid attributeId,
        CancellationToken cancellationToken);
    Task<List<CvAttribute>> GetAvailableAttributesAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> RemoveAttributeAsync(Guid userid, Guid attributeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AttributeLookupResult>> SearchAvailableAttributesAsync(
        Guid userId,
        string prefix,
        CancellationToken cancellationToken);
    
    Task<IReadOnlyList<AttributeLookupResult>> GetRecentlyUsedAttributesAsync(Guid userId, CancellationToken cancellationToken);
}