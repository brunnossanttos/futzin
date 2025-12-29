namespace Futzin.Api.Application.DTOs;

public record RegisterDto(
    string Name,
    string Email,
    string Password,
    string? Phone
);

public record LoginDto(
    string Email,
    string Password
);

public record UpdateUserDto(
    string? Name,
    string? Phone,
    string? PhotoUrl
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

public record UserResponseDto(
    int Id,
    string Name,
    string Email,
    string? Phone,
    string? PhotoUrl,
    DateTime CreatedAt
);

public record AuthResponseDto(
    string Token,
    UserResponseDto User
);

public record OAuthLoginDto(
    string Provider,
    string IdToken
);
