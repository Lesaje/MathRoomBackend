using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathRoom.Backend.Controllers;

[Route("groupes/{groupId:int}/students/{studentId:int}")]
public class GroupsStudentsController : ApiController
{
    private readonly DataContext _context;
    
    public GroupsStudentsController(DataContext dataContext) : base(dataContext)
    {
        _context = dataContext;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Problem>>> GetProblems(int groupId, int studentId)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null)
        {
            return BadRequest("Group with such Id does not exist");
        }
        
        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == studentId);
        if (student is null)
        {
            return BadRequest("Student with such Id does not exist");
        }
        
        var tags = _context.Groups.Where(g => g.Id == groupId).Select(g => g.Tags).Single();
        
        var problems = await _context.Problems.Where(
            p => tags.Contains(p.Tag) && !student.SolvedProblems.Contains(p))
            .Take(10).ToListAsync();
        
        return Ok(problems);
    }
}