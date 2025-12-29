using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Domain.Entities;

public class PeladaAttendance
{
    public int Id { get; set; }
    public int PeladaId { get; set; }
    public int UserId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Pending;
    public DateTime? ResponseDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pelada Pelada { get; set; } = null!;
    public User User { get; set; } = null!;
}
