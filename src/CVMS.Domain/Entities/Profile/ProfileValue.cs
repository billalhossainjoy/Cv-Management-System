namespace CVMS.Domain.Entities;

public class ProfileValue: BaseEntity
{
    public Guid AttributeId { get; set; }
    public Guid ProfileId { get; set; }
    public string? Value  { get; set; }
    public uint Version { get; set; }
    public Profile Profile { get; set; } = null!;
    public CvAttribute Attribute { get; set; } = null!;
}
