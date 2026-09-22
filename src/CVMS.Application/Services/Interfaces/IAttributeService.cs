using CVMS.Application.Attribute;

namespace CVMS.Application.Services.Interfaces;

public interface IAttributeService
{
    Task<IReadOnlyList<AttributeListItem>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<AttributeDetails?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<AttributeResultStatus> CreateAsync(
        CreateAttributeRequest request,
        CancellationToken cancellationToken);

    Task<AttributeResultStatus> UpdateAsync(
        Guid id,
        UpdateAttributeRequest request,
        CancellationToken cancellationToken);

    Task<AttributeResultStatus> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}