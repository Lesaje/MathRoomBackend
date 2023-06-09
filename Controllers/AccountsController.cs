using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Extensions;
using MathRoom.Backend.Models.Identity;
using MathRoom.Backend.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathRoom.Backend.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountsController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("/login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var managedUser = await _userManager.FindByEmailAsync(request.Email);
        
        if (managedUser == null)
        {
            return BadRequest("Bad credentials");
        }
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
        
        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }
        
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        
        if (user is null)
            return Unauthorized();

        var roleIds = await _context.UserRoles.Where(r => r.UserId == user.Id).
            Select(x => x.RoleId).ToListAsync();
        
        var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();
        
        var accessToken = _tokenService.CreateToken(user, roles);
        
        await _context.SaveChangesAsync();
        
        return Ok(new AuthResponse
        {
            Roles = AccountExtensions.GetUserRoles(_context, user.Email!),
            Username = user.UserName!,
            Email = user.Email!,
            Token = accessToken
        });
    }
    
    [HttpPost("/register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState.IsValid");
            return BadRequest(request);
        }
        
        var user = new ApplicationUser
        {
            FirstName = request.FirstName, 
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            UserName = request.Email
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        if (!result.Succeeded)
        {
            Console.WriteLine("CreateAsync");
            return BadRequest(request);
        }
        
        var findUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (findUser == null) throw new Exception($"User {request.Email} not found");

        switch (request.UserRole)
        {
            case RoleConsts.Teacher:
                await _userManager.AddToRoleAsync(findUser, RoleConsts.Teacher);
                break;
            case RoleConsts.Student:
                await _userManager.AddToRoleAsync(findUser, RoleConsts.Student);
                break;
            default:
                return BadRequest(request);
        }
        
        await _context.SaveChangesAsync();
            
        return await Authenticate(new AuthRequest
        {
            Email = request.Email,
            Password = request.Password
        });
    }
    

}