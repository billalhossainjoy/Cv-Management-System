namespace CVMS.Application.Positions;

public sealed record PositionListItem(
    Guid Id,
    string Title,
    string ShortDescription,
    int MaximumProjects,
    int AttributeCount);

