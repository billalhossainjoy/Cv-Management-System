namespace CVMS.Application.Positions;

public sealed record CreatePositionRequest(
    string Title,
    string ShortDescription,
    int MaximumProjects,
    IReadOnlyList<PositionAttributeRequest> Attributes);
    