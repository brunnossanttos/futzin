using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Application.DTOs;

public record CreatePeladaDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime Date { get; init; }
    public required string Location { get; init; }
    public required FieldType FieldType { get; init; }
    public required decimal Price { get; init; }
    public required int MaxPlayers { get; init; }
    public bool IsPublic { get; init; } = false;
}

public record UpdatePeladaDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? Date { get; init; }
    public string? Location { get; init; }
    public FieldType? FieldType { get; init; }
    public decimal? Price { get; init; }
    public int? MaxPlayers { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsPublic { get; init; }
}

public record PeladaResponseDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public required string Location { get; init; }
    public FieldType FieldType { get; init; }
    public string FieldTypeName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int MaxPlayers { get; init; }
    public int CurrentPlayers { get; init; }
    public bool IsActive { get; init; }
    public bool IsPublic { get; init; }
    public int CreatedById { get; init; }
    public required string CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record PeladaDetailDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public required string Location { get; init; }
    public FieldType FieldType { get; init; }
    public string FieldTypeName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int MaxPlayers { get; init; }
    public int CurrentPlayers { get; init; }
    public bool IsActive { get; init; }
    public bool IsFull { get; init; }
    public bool IsPublic { get; init; }
    public string? InviteLink { get; init; }
    public int CreatedById { get; init; }
    public required string CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<ParticipantInfoDto> Participants { get; init; } = new();
    public List<TeamInfoDto> Teams { get; init; } = new();
    public List<InviteInfoDto> Invites { get; init; } = new();
}

public record ParticipantInfoDto
{
    public int UserId { get; init; }
    public required string UserName { get; init; }
    public string? PhotoUrl { get; init; }
    public bool HasPaid { get; init; }
    public int? TeamId { get; init; }
    public string? TeamName { get; init; }
    public DateTime JoinedAt { get; init; }
}

public record TeamInfoDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
    public int PlayerCount { get; init; }
    public List<string> PlayerNames { get; init; } = new();
}

public record UpdatePaymentStatusDto
{
    public required bool HasPaid { get; init; }
}

public record InviteUserDto
{
    public required string Email { get; init; }
}

public record InviteInfoDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public required string UserName { get; init; }
    public required string UserEmail { get; init; }
    public string? PhotoUrl { get; init; }
    public bool IsAccepted { get; init; }
    public DateTime InvitedAt { get; init; }
    public DateTime? AcceptedAt { get; init; }
}

public record JoinWithTokenDto
{
    public required string InviteToken { get; init; }
}
