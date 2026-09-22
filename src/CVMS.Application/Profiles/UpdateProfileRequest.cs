namespace CVMS.Application.Profiles;

public sealed record UpdateProfileRequest(
    uint Version,
    IReadOnlyCollection<UpdateProfileValueRequest> Values);

public sealed record UpdateProfileValueRequest(
    Guid AttributeId,
    string? Value);