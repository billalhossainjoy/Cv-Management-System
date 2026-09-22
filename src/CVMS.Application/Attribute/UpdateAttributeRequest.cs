using CVMS.Domain.Attributes;

namespace CVMS.Application.Attribute;

public sealed record UpdateAttributeRequest(
    string Name,
    string Description,
    AttributeCategory Category,
    AttributeType Type);