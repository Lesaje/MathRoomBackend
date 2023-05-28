using MathRoom.Backend.Data.Entities;

namespace MathRoom.Backend.Models.Identity;

public class GroupResponse : BaseGroup
{
    public int Id { get; set; }
    public List<BaseAccountResponse> GroupUsers { get; set; } = null!;
}