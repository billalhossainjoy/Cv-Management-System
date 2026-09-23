namespace CVMS.Application.Positions;

public sealed record PositionAttributeRequest(
    Guid AttributeId,
    bool IsRequired,
    int DisplayOrder);
    