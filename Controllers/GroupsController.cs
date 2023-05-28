using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Extensions;
using MathRoom.Backend.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace MathRoom.Backend.Controllers;

[Route("groups")]
public class GroupsController : ControllerBase
{
    private readonly DataContext _context;
    
    public GroupsController(DataContext context)
    {
        _context = context;
    }
    
    [HttpPost(Name = "Create Group")]
    public async Task<IActionResult> CreateGroup([FromBody] GroupRequest request)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model state is invalid");
            return BadRequest(ModelState);
        }
        
        var group = new Group
        {
            Name = request.Name,
            Tags = request.Tags
        };
        
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet("user/{id:int}", Name = "Get Groups")]
    public async Task<ActionResult<List<GroupResponse>>> GetGroups(int id)
    {
        //return all groups where the user is a member
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return BadRequest();
        
        var groups = await _context.Groups.Where(g => g.ApplicationUsers.Contains(user)).ToListAsync();
        var users = await _context.Groups.Where(g => g.ApplicationUsers.Contains(user)).Select(g => g.ApplicationUsers).ToListAsync();

        
        
        var response = new List<GroupResponse>();

        //write a for loop with i index instead of foreach
        for (int i = 0; i < groups.Count; i++)
        {
            response.Add(new GroupResponse
            {
                Id = groups[i].Id,
                Name = groups[i].Name,
                Tags = groups[i].Tags,
                GroupUsers = new List<BaseAccountResponse>()
            });
            
            foreach (var user1 in users[i])
            {
                response[i].GroupUsers.Add(new BaseAccountResponse {
                    Roles = AccountExtensions.GetUserRoles(_context, user1.Email!),
                    Username = user1.UserName,
                    Email = user1.Email
                });
            }
        }

        return Ok(response);
    }
    
    [HttpGet("{id:int}", Name = "Get Group")]
    public async Task<ActionResult<GroupResponse>> GetGroup(int id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group is null)
            return BadRequest();
        
        var response = new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Tags = group.Tags,
            GroupUsers = new List<BaseAccountResponse>()
        };

        foreach (var user in group.ApplicationUsers)
        {
            response.GroupUsers.Add(new BaseAccountResponse {
                Roles = AccountExtensions.GetUserRoles(_context, user.Email!),
                Username = user.UserName,
                Email = user.Email
            });
        }

        return Ok(response);
    }
    
    
    
}