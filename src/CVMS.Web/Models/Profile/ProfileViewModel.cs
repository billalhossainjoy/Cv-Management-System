using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Profile;

public class ProfileAttributeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Value { get; set; }
}

public class ProfileViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
    
    [Display(Name = "Address")]
    public string? Location { get; set; } 
    
    public string? PhotoUrl { get; set; }


    public List<ProfileAttributeViewModel> Attributes { get; set; } = new();

    public List<ProjectViewModel> Projects { get; set; } = new();
}

public class ProjectViewModel
{
    public string Name { get; set; } = string.Empty;

    public string Period { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Technologies { get; set; } = new();
}