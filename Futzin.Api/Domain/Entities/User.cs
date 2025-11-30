namespace Futzin.Api.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Pelada> PeladasCreated { get; set; } = new List<Pelada>();
    public ICollection<PeladaParticipant> Participations { get; set; } = new List<PeladaParticipant>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
