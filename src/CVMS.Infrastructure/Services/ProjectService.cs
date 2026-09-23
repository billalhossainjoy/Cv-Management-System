using CVMS.Application.Projects;
using CVMS.Application.Services;
using CVMS.Application.Services.Interfaces;
using CVMS.Domain.Entities;
using CVMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVMS.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetProjectsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Projects
            .Where(p => p.Profile.UserId == userId)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectResultStatus> CreateProjectAsync(
        Guid userId,
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        if (request.EndDate is not null &&
            request.EndDate < request.StartDate)
        {
            return ProjectResultStatus.InvalidDateRange;
        }

        var profileId = await _context.Profiles
            .Where(p => p.UserId == userId)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (profileId is null)
        {
            return ProjectResultStatus.NotFound;
        }

        var project = new Project
        {
            ProfileId = profileId.Value,
            Name = request.Name.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description.Trim()
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(cancellationToken);

        return ProjectResultStatus.Success;
    }

    public async Task<Project?> GetProjectAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(
                p =>
                    p.Id == projectId &&
                    p.Profile.UserId == userId,
                cancellationToken);
    }

    public async Task<ProjectResultStatus> UpdateProjectAsync(
        Guid userId,
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(
                p =>
                    p.Id == projectId &&
                    p.Profile.UserId == userId,
                cancellationToken);

        if (project is null)
        {
            return ProjectResultStatus.NotFound;
        }

        if (request.EndDate is not null &&
            request.EndDate < request.StartDate)
        {
            return ProjectResultStatus.InvalidDateRange;
        }

        project.Name = request.Name.Trim();
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Description = request.Description.Trim();
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ProjectResultStatus.Success;
    }

    public async Task<ProjectResultStatus> DeleteProjectAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(
                p =>
                    p.Id == projectId &&
                    p.Profile.UserId == userId,
                cancellationToken);

        if (project is null)
        {
            return ProjectResultStatus.NotFound;
        }

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync(cancellationToken);

        return ProjectResultStatus.Success;
    }
}