using CVMS.Application.Profiles;
using CVMS.Domain.Entities;

namespace CVMS.Application.Services;

public interface IProfileService
{
    Task<Profile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<UpdateProfileResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
}