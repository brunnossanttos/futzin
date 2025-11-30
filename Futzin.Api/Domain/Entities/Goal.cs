namespace Futzin.Api.Domain.Entities;

public class Goal
{
    public int Id { get; set; }
    public int PeladaId { get; set; }
    public int ScorerId { get; set; }
    public int TeamId { get; set; }
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    
    // Navigation properties
    public Pelada Pelada { get; set; } = null!;
    public User Scorer { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
