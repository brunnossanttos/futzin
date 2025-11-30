namespace Futzin.Api.Domain.Entities;

public class Pelada
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsActive { get; set; } = true;
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User CreatedBy { get; set; } = null!;
    public ICollection<PeladaParticipant> Participants { get; set; } = new List<PeladaParticipant>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
