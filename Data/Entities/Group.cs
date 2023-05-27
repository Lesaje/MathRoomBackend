namespace MathRoom.Backend.Data.Entities;

public class Group
{
    public Guid Id { get; set; }
    public List<ApplicationUser> GroupUsers = new List<ApplicationUser>();
    public string GroupName = "";
    public List<string> Tags = new List<string>();
}