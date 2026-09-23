using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Position;

public class CreatePositionViewModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string ShortDescription { get; set; } = string.Empty;

    [Range(0, 100)]
    public int MaximumProjects { get; set; }

    public List<PositionAttributeSelectionViewModel> Attributes { get; set; }
        = new();
}