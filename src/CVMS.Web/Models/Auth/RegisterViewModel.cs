using System.ComponentModel.DataAnnotations;

namespace CVMS.Web.Models.Auth;

public sealed class RegisterViewModel
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
    
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }= string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }= string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }= string.Empty;
}