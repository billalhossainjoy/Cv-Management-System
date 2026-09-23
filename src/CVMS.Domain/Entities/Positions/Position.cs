namespace CVMS.Domain.Entities.Positions;

public class Position: BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public int MaximumProjects { get; set; }

    public ICollection<PositionAttribute> Attributes { get; set; }
        = new List<PositionAttribute>();
    
    public ICollection<TechnologyTag> TechnologyTags { get; set; }
        = new List<TechnologyTag>();
}