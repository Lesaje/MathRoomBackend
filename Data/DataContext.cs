using MathRoom.Backend.Data.Entities;
using MathRoom.Backend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Group = MathRoom.Backend.Data.Entities.Group;

namespace MathRoom.Backend.Data;

public sealed class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public DataContext (DbContextOptions<DataContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    public DbSet<Group> Groups { get; set; } = null!;
}