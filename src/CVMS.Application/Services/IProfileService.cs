using CVMS.Domain.Entities;

namespace CVMS.Application.Services;

public interface IProfileService
{
    Task<Profile?> GetProfileAsync(Guid profileId, CancellationToken cancellationToken);
}