using CVMS.Domain.Entities.Positions;

namespace CVMS.Domain.Entities;

public class TechnologyTag: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Project> Projects { get; set; }
        = new List<Project>();

    public ICollection<Position> Positions { get; set; }
        = new List<Position>();
    
}