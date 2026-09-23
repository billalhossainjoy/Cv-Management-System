namespace CVMS.Domain.Entities.Positions;

public class PositionAttribute
{
    public Guid PositionId { get; set; }
    public Position Position { get; set; } = null!;

    public Guid AttributeId { get; set; }
    public CvAttribute Attribute { get; set; } = null!;

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }
}