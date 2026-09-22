using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Profile;

public class ProfileViewModel
{
    public uint Version { get; set; }
    public Guid Id { get; set; }

    public List<ProfileValueViewModel> Values { get; set; } = new();
}

public class ProfileValueViewModel
{
    public Guid AttributeId { get; set; }

    public string AttributeName { get; set; } = string.Empty;

    public string AttributeType { get; set; } = string.Empty;

    public string? Value { get; set; }
}
