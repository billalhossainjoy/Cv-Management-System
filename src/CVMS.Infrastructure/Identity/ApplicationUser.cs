using Microsoft.AspNetCore.Identity;

namespace CVMS.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public bool IsBlocked { get; set; }
    
    public DateTime? DeletedAt {get; set; }
}