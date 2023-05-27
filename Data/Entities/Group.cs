namespace MathRoom.Backend.Data.Entities;

public class Group
{
    public int Id { get; set; }
    public string GroupName { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
}