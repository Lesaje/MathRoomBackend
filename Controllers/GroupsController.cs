using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MathRoom.Backend.Controllers;

[Route("groupes")]
public class GroupsController : ApiController
{
    private readonly DataContext _context;
    
    public GroupsController(DataContext dataContext) : base(dataContext)
    {
        _context = dataContext;
    }
    
    [Authorize(Roles = "Teacher")]
    [HttpPost(Name = "Create Group")]
    public async Task<IActionResult> CreateGroup([FromBody] GroupRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var group = new Group
        {
            GroupName = request.GroupName,
            Tags = request.Tags
        };
        
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet(Name = "Get Groups")]
    public async Task<List<Group>> GetGroups()
    {
        return await _context.Groups.ToListAsync();
    }

    [HttpPost("{id:guid}", Name = "Join Group")]
    public async Task<IActionResult> JoinGroup([FromBody] string userName, Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        
        if (user is null)
            return NotFound();
        
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        
        if (group is null)
            return NotFound();
        
        group.GroupUsers.Add(user);
        user.Groups.Add(group);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}