using System.ComponentModel.DataAnnotations;

namespace MathRoom.Backend.Models.Identity;

public class RegisterRequest
{
    [Required] 
    [Display(Name = "User Role")] 
    public string UserRole { get; set; } = null!;
    
    [Required] 
    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;
 
    //how about hashing
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;
    
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;
    
    [Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }
}