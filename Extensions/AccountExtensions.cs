using System.Runtime.InteropServices.JavaScript;
using MathRoom.Backend.Data;
using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MathRoom.Backend.Extensions;



public class AccountExtensions
{
    public static List<string?> GetUserRoles(DataContext _context, string Email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == Email);
        
        var roleIds = _context.UserRoles.Where(r => r.UserId == user.Id).
            Select(x => x.RoleId).ToList();
        
        var roles = _context.Roles.Where(x => roleIds.Contains(x.Id)).ToList();
        var rolesNames = roles.Select(x => x.Name).ToList();

        return rolesNames;
    }
}