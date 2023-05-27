using System.ComponentModel.DataAnnotations;

namespace MathRoom.Backend.Models.Identity;

public class ProblemRefreshRequest
{
    [Required] 
    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;
    [Required] 
    [Display(Name = "Solved Problems")] 
    public Dictionary<int, bool> TriedProblems { get; set; } = null!; 
}