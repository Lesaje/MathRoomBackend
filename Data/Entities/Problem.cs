namespace MathRoom.Backend.Data.Entities;

public class Problem
{
    public int Id { get; set; }
    public string Tag { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public string Path { get; set; } = null!;
    
}