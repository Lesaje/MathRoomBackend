namespace MathRoom.Backend.Models.Identity;

public class AuthResponse : BaseAccountResponse
{
    public string Token { get; set; } = null!;
}