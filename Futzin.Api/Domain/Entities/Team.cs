namespace Futzin.Api.Domain.Entities;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int PeladaId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Pelada Pelada { get; set; } = null!;
    public ICollection<PeladaParticipant> Players { get; set; } = new List<PeladaParticipant>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
