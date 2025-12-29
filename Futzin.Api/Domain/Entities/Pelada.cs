using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Domain.Entities;

public class Pelada
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public decimal Price { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsActive { get; set; } = true;

    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? DayOfWeek { get; set; }
    public int? DayOfMonth { get; set; }

    public int? GroupId { get; set; }

    public string InviteToken { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User CreatedBy { get; set; } = null!;
    public PeladaGroup? Group { get; set; }
    public ICollection<PeladaParticipant> Participants { get; set; } = new List<PeladaParticipant>();
    public ICollection<PeladaAttendance> Attendances { get; set; } = new List<PeladaAttendance>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<PeladaInvite> Invites { get; set; } = new List<PeladaInvite>();
}
