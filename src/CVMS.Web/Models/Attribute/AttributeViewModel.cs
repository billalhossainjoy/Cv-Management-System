using System.ComponentModel.DataAnnotations;
using CVMS.Domain.Attributes;

namespace CVMS.Web.Models.Attribute;

public class CreateAttributeViewModel
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public AttributeCategory Category { get; set; }

    [Required]
    public AttributeType Type { get; set; }
}

public class EditAttributeViewModel
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public AttributeCategory Category { get; set; }

    [Required]
    public AttributeType Type { get; set; }

    public bool IsBuiltIn { get; set; }
}