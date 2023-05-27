using MathRoom.Backend.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MathRoom.Backend.Services.Identity;

public interface ITokenService
{
    string CreateToken(ApplicationUser user, List<IdentityRole<long>> role);
}