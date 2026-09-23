using CVMS.Application.TechnologyTags;

namespace CVMS.Application.Services.Interfaces;

public interface ITechnologyTagService
{
    Task<IReadOnlyList<TechnologyTagLookup>> SearchAsync(
        string prefix,
        CancellationToken cancellationToken);

    Task<TechnologyTagLookup> GetOrCreateAsync(
        string name,
        CancellationToken cancellationToken);
}