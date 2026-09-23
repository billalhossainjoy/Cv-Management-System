namespace CVMS.Application.Positions;

public sealed record UpdatePositionRequest(
    string Title,
    string ShortDescription,
    int MaximumProjects,
    IReadOnlyList<PositionAttributeRequest> Attributes);