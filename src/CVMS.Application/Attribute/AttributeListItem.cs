namespace CVMS.Application.Attribute;


public sealed record AttributeListItem(
    Guid Id,
    string Name,
    string Description,
    string Category,
    string Type,
    bool IsBuiltIn);