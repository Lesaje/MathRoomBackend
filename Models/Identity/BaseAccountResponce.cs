namespace MathRoom.Backend.Models.Identity;

public class BaseAccountResponse
{
    public List<string?> Roles { get; set; } = new List<string?>();
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}