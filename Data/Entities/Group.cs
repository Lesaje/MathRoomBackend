using Microsoft.EntityFrameworkCore;

namespace MathRoom.Backend.Data.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    
    public class Statistics
    {
        public double AverageScore { get; set; }
        public int OverallSolvedProblems { get; set; }
        public int OverallTriedProblems { get; set; }
    }
    
    public Statistics GetStatistics(ApplicationUser user)
    {
        
        var solvedProblems = user.ProblemsProgress.Count(p => p.IsSolved);
        var overallProblems = user.ProblemsProgress.Count;
        
        return new Statistics
        {
            AverageScore = solvedProblems / double.Parse(overallProblems.ToString()),
            OverallSolvedProblems = solvedProblems,
            OverallTriedProblems = overallProblems
        };
    }
}