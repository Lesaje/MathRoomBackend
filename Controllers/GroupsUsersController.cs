using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Models.Identity;
using MathRoom.Backend.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathRoom.Backend.Controllers;

[Route("groups/{groupId:int}/users/{userId:int}")]
public class GroupsUsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    
    public GroupsUsersController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    [HttpGet("student/", Name = "Get Problems")]
    public async Task<ActionResult<List<Problem>>> GetProblems(int groupId, int userId)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null)
        {
            return BadRequest("Group with such Id does not exist");
        }
        
        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (student is null)
        {
            return BadRequest("Student with such Id does not exist");
        }
        
        var groupTags = group.Tags;
        
        //select all problems with tags from groupTags
        var problems = await _context.Problems.Where(p => groupTags.Contains(p.Tag)).ToListAsync();
        
        //select all problems that student has already solved
        var solvedProblems = student.ProblemsProgress.Where(pp => pp.IsSolved).Select(pp => pp.ProblemId).ToList();
        
        //remove all solved problems from the list of problems
        problems.RemoveAll(p => solvedProblems.Contains(p.Id));
        
        return Ok(problems.Take(10));
    }
    
    [HttpPost("student/", Name = "Refresh List Of Solved Problems")]
    public async Task<IActionResult> RefreshListOfSolvedProblems(int groupId, int userId, Dictionary<int, bool> request)
    {
        var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (student == null)
        {
            return BadRequest("Bad credentials");
        }
        //find list af all tags in this group
        var tags = _context.Groups.Where(g => g.Id == groupId).Select(g => g.Tags).Single();
        //find all tags of problems with this id's
        var problemTags = await _context.Problems.Where(p => request.Keys.Contains(p.Id)).Select(p => p.Tag).ToListAsync();
        //check, if all problemTags exists in tags
        if (!problemTags.All(tags.Contains))
        {
            return BadRequest("Bad credentials");
        }

        foreach (var keypair in request)
        {
            
            if (keypair.Value == true)
            {
                var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == keypair.Key);
                if (problem == null)
                {
                    return BadRequest("No such problem exists");
                }
                student.ProblemsProgress.Add(new IsProblemSolved {
                    ProblemId = problem.Id, 
                    IsSolved = true
                });
            }
            else
            {
                var problem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == keypair.Key);
                if (problem == null)
                {
                    return BadRequest("No such problem exists");
                }
                student.ProblemsProgress.Add(new IsProblemSolved {
                    ProblemId = problem.Id, 
                    IsSolved = false
                });
            }
        }
        
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPatch("join", Name = "Join Group")]
    public async Task<IActionResult> JoinGroup(int groupId, int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
            return NotFound();
        
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        
        if (group is null)
            return NotFound();
        
        group.ApplicationUsers.Add(user);
        user.Groups.Add(group);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    [HttpPatch("leave", Name = "Leave Group")]
    public async Task<IActionResult> LeaveGroup(int groupId, int userId)
    {
        var user = await _context.Users.Include(u => u.Groups).
            FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return NotFound();

        var group = await _context.Groups.Include(g => g.ApplicationUsers).
            FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
            return NotFound();
        
        //check if this user are the member of this group using _context.
        if (!await _context.Groups.AnyAsync(g => g.Id == groupId && g.ApplicationUsers.Any(u => u.Id == userId)))
            return BadRequest();

        group.ApplicationUsers.Remove(user);
        user.Groups.Remove(group);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpDelete("teacher/", Name = "Delete Group")]
    public async Task<IActionResult> DeleteGroup(int groupId, int userId)
    {
        //check if this teacher are the member of this group
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return BadRequest();
        
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null)
            return BadRequest();
        
        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPatch("teacher/", Name = "Update Group")]
    public async Task<IActionResult> UpdateGroup(int groupId, int userId, [FromBody] BaseGroup request)
    {
        var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (teacher is null)
            return BadRequest();
        
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        
        if (group is null)
            return BadRequest();
        
        group.Name = request.Name;
        group.Tags = request.Tags;
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    [HttpPatch("teacher/{studentId:int}", Name = "Delete User From Group")]
    public async Task<IActionResult> DeleteUserFromGroup(int groupId, int userId, int studentId)
    {
        var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (teacher is null)
            return BadRequest();
        
        var group = await _context.Groups.Include(g => g.ApplicationUsers).
            FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null)
            return BadRequest();
        
        var student = await _context.Users.Include(u => u.Groups).
            FirstOrDefaultAsync(u => u.Id == studentId);
        if (student is null)
            return BadRequest();
        
        group.ApplicationUsers.Remove(student);
        student.Groups.Remove(group);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet("statistics/", Name = "Get Student Statistics")]
    public async Task<IActionResult> GetStudentStatistics(int groupId, int userId)
    {
        var user = await _context.Users.Include(u => u.ProblemsProgress).FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
            return BadRequest();
        
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
        
        if (group is null)
            return BadRequest();
        
        
        return Ok(group.GetStatistics(user));
    }
    
}