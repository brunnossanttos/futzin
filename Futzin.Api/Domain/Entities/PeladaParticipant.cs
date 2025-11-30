namespace Futzin.Api.Domain.Entities;

public class PeladaParticipant
{
    public int Id { get; set; }
    public int PeladaId { get; set; }
    public int UserId { get; set; }
    public bool HasPaid { get; set; } = false;
    public int? TeamId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Pelada Pelada { get; set; } = null!;
    public User User { get; set; } = null!;
    public Team? Team { get; set; }
}
