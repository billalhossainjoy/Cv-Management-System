using CVMS.Domain.Attributes;

namespace CVMS.Application.Attribute;

public sealed record AttributeDetails(
    Guid Id,
    string Name,
    string Description,
    AttributeCategory Category,
    AttributeType Type,
    bool IsBuiltIn);