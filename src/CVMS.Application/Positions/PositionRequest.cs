namespace CVMS.Application.Positions;

public enum PositionResultStatus
{
    Success,
    InvalidAttribute,
    DuplicateAttribute,
    InvalidMaximumProjects
}

public sealed record PositionListItem(
    Guid Id,
    string Title,
    string ShortDescription,
    int MaximumProjects,
    int AttributeCount);
    
    
public sealed record CreatePositionRequest(
    string Title,
    string ShortDescription,
    int MaximumProjects,
    IReadOnlyList<PositionAttributeRequest> Attributes);