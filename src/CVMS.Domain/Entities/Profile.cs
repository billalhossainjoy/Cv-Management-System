namespace CVMS.Domain.Entities;

public class Profile: BaseEntity
{
    public Guid UserId { get; set; }
    public uint Version { get; set; }
    public List<ProfileValue> Values { get; set; } = new();
}