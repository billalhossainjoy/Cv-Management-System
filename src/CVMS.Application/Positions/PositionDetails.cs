namespace CVMS.Application.Positions;

public sealed record PositionDetails(
    Guid Id,
    string Title,
    string ShortDescription,
    int MaximumProjects,
    IReadOnlyList<PositionAttributeDetails> Attributes);
    
public sealed record PositionAttributeDetails(
    Guid AttributeId,
    bool IsRequired,
    int DisplayOrder);