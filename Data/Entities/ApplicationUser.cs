using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MathRoom.Backend.Data.Entities;

public class ApplicationUser : IdentityUser<long>
{
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string? MiddleName { get; set; }
    public List<Group> Groups { get; set; } = new List<Group>();
    public List<Problem> SolvedProblems { get; set; } = new List<Problem>();
    public int HowManyProblemsWasTried { get; set; }
}