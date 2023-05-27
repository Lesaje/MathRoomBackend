using Microsoft.AspNetCore.Identity;

namespace MathRoom.Backend.Data.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    
    public List<Group> Groups = new List<Group>();
}