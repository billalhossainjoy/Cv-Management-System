using CVMS.Application.Projects;
using CVMS.Domain.Entities;

namespace CVMS.Application.Services.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<Project>> GetProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<ProjectResultStatus> CreateProjectAsync(
        Guid userId,
        CreateProjectRequest request,
        CancellationToken cancellationToken);
    
    Task<Project?> GetProjectAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken);

    Task<ProjectResultStatus> UpdateProjectAsync(
        Guid userId,
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken);

    Task<ProjectResultStatus> DeleteProjectAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken);
    
}