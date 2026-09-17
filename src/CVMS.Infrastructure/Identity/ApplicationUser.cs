using Microsoft.AspNetCore.Identity;

namespace CVMS.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}";
    
    public string? ProfileImageUrl { get; set; }
    
    public bool IsBlocked { get; set; }
    
    public DateTime? DeletedAt {get; set; }


    
}