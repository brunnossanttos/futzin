namespace Futzin.Api.Domain.Entities;

public class UserStats
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public int TotalPeladas { get; set; } = 0;
    public int TotalGoals { get; set; } = 0;
    public int TotalWins { get; set; } = 0;
    public int TotalLosses { get; set; } = 0;
    public int TotalDraws { get; set; } = 0;
    public int TotalNoShows { get; set; } = 0;

    public decimal WinRate { get; set; } = 0;
    public decimal GoalsPerGame { get; set; } = 0;
    public decimal AttendanceRate { get; set; } = 0;

    public int CurrentWinStreak { get; set; } = 0;
    public int BestWinStreak { get; set; } = 0;
    public DateTime? LastPeladaDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
