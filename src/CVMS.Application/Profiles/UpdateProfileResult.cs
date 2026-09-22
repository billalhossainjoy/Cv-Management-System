namespace CVMS.Application.Profiles;

public enum UpdateProfileStatus
{
    Success,
    NotFound,
    InvalidAttribute,
    Conflict
}

public sealed record UpdateProfileResult(
    UpdateProfileStatus Status,
    uint? Version = null);