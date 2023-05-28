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
    
    public async Task<Statistics> GetStatistics(DataContext _context, ApplicationUser user)
    {
        //select solved problems which Tag is in Tags
        var tags = Tags;
        var solvedProblems = await _context.Problems.Where(
            p => tags.Contains(p.Tag) && user.SolvedProblems.Contains(p))
            .ToListAsync();
        
        //select tried problems which Tag is in Tags
        var unsolvedProblems = await _context.Problems.Where(
            p => tags.Contains(p.Tag) && user.UnSolvedProblems.Contains(p))
            .ToListAsync();
        
        return new Statistics
        {
            AverageScore = solvedProblems.Count / double.Parse((unsolvedProblems.Count + solvedProblems.Count).ToString()),
            OverallSolvedProblems = solvedProblems.Count,
            OverallTriedProblems = unsolvedProblems.Count + solvedProblems.Count
        };
    }
}