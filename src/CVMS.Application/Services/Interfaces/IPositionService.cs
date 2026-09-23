using CVMS.Application.Positions;

namespace CVMS.Application.Services.Interfaces;

public interface IPositionService
{
    Task<IReadOnlyList<PositionListItem>> GetAllAsync(
        CancellationToken cancellationToken);
    
    Task<PositionResultStatus> CreateAsync(
        CreatePositionRequest request,
        CancellationToken cancellationToken);
    
    Task<PositionDetails?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<PositionResultStatus> UpdateAsync(
        Guid id,
        UpdatePositionRequest request,
        CancellationToken cancellationToken);

    Task<PositionResultStatus> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<PositionResultStatus> DuplicateAsync(
        Guid id,
        CancellationToken cancellationToken);
}