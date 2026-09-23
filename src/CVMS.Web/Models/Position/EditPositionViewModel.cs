namespace CVMS.Web.Models.Position;


public class EditPositionViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public int MaximumProjects { get; set; }

    public List<PositionAttributeSelectionViewModel> Attributes { get; set; } = [];
}