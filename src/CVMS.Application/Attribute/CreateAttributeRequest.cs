using CVMS.Domain.Attributes;

namespace CVMS.Application.Attribute;

public enum AttributeResultStatus
{
    Success,
    NotFound,
    DuplicateName,
    BuiltInProtected
}

public sealed record CreateAttributeRequest(
    string Name,
    string Description,
    AttributeCategory Category,
    AttributeType Type);