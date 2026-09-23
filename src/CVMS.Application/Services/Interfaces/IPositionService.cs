using CVMS.Application.Positions;

namespace CVMS.Application.Services.Interfaces;

public interface IPositionService
{
    Task<IReadOnlyList<PositionListItem>> GetAllAsync(
        CancellationToken cancellationToken);
    
    Task<PositionResultStatus> CreateAsync(
        CreatePositionRequest request,
        CancellationToken cancellationToken);
    
}