namespace Futzin.Api.Domain.Entities;

public class GroupMember
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public bool IsAdmin { get; set; } = false;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public PeladaGroup Group { get; set; } = null!;
    public User User { get; set; } = null!;
}
