using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Profile;

public class ProfileViewModel
{
    public uint Version { get; set; }
    public Guid Id { get; set; }

    public List<ProfileValueViewModel> Values { get; set; } = new();
    public List<AvailableAttributeViewModel> AvailableAttributes { get; set; } = new();
}

public class AvailableAttributeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class ProfileValueViewModel
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; } = string.Empty;

    public string AttributeType { get; set; } = string.Empty;

    public string? Value { get; set; }

    public bool IsBuiltIn { get; set; }
}
