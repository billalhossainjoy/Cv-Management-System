namespace CVMS.Web.Models.Project;

public class ProjectListItemViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}