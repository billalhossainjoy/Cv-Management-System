using CVMS.Domain.Attributes;

namespace CVMS.Domain.Entities;

public class CvAttribute: BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AttributeCategory Category { get; set; }

    public AttributeType Type { get; set; }

    public bool IsBuiltIn { get; set; }
    public uint Version { get; set; }

    public List<ProfileValue> ProfileValues { get; set; } = new();
    
    public int DisplayOrder { get; set; }

    public CvAttribute(){}
    
    public CvAttribute(
        string name,
        string description,
        AttributeCategory category,
        AttributeType type,
        bool isBuiltIn = false)
    {
        SetName(name);

        Description = description.Trim();
        Category = category;
        Type = type;
        IsBuiltIn = isBuiltIn;
    }
    
    public void Update(
        string name,
        string description,
        AttributeCategory category,
        AttributeType type)
    {
        SetName(name);

        Description = description.Trim();
        Category = category;
        Type = type;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Attribute name is required.",
                nameof(name));
        }

        Name = name.Trim();
    }
}