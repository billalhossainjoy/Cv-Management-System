using CVMS.Domain.Entities.Profiles;
namespace CVMS.Domain.Entities;


public class Project: BaseEntity
{
    public Guid ProfileId { get; set; }

    public Profile Profile { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public ICollection<TechnologyTag> TechnologyTags { get; set; }
        = new List<TechnologyTag>();
}