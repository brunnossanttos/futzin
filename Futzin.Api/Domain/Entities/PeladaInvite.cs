namespace Futzin.Api.Domain.Entities;

public class PeladaInvite
{
    public int Id { get; set; }
    public int PeladaId { get; set; }
    public int InvitedUserId { get; set; }
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }

    public Pelada Pelada { get; set; } = null!;
    public User InvitedUser { get; set; } = null!;
}
