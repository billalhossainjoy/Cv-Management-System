using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Project;

public class EditProjectViewModel
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}