namespace MathRoom.Backend.Models.Identity;

public class GroupRequest
{
    public string GroupName { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
}