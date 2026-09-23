namespace CVMS.Web.Models.Position;

public class PositionAttributeSelectionViewModel
{
    public Guid AttributeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool Selected { get; set; }

    public bool IsRequired { get; set; }
}