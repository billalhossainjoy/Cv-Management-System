using System.ComponentModel.DataAnnotations;

namespace CVMS.Domain.Attributes;

public enum AttributeCategory
{
    Certification,
    [Display(Name = "Domain Knowledge")]
    DomainKnowledge,
    [Display(Name = "Personal Information")]
    PersonalInformation,
    [Display(Name = "Soft Skills")]
    SoftSkills
}