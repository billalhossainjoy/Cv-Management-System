namespace CVMS.Application.Projects;

public sealed record CreateProjectRequest(
    string Name,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Description);
    
public sealed record UpdateProjectRequest(
    string Name,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Description);