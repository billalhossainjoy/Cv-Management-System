using CVMS.Application.Projects;
using CVMS.Domain.Entities;

namespace CVMS.Application.Services;

public interface IProjectService
{
    Task<IReadOnlyList<Project>> GetProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> CreateProjectAsync(
        Guid userId,
        CreateProjectRequest request,
        CancellationToken cancellationToken);
    
}