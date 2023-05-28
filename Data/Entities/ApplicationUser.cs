using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    public List<IsProblemSolved> ProblemsProgress { get; set; } = null!;
}

public record IsProblemSolved
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public bool IsSolved { get; set; }
}