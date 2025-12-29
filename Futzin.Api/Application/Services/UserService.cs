using Futzin.Api.Application.DTOs;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> OAuthLoginAsync(OAuthLoginDto dto);
    Task<UserResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
    Task ChangePasswordAsync(int id, ChangePasswordDto dto);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public UserService(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _authService.HashPassword(dto.Password),
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow
        };

        user = await _userRepository.CreateAsync(user);

        var token = _authService.GenerateJwtToken(user);

        return new AuthResponseDto(
            token,
            MapToUserResponse(user)
        );
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user == null || !_authService.VerifyPassword(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = _authService.GenerateJwtToken(user);

        return new AuthResponseDto(
            token,
            MapToUserResponse(user)
        );
    }

    public async Task<AuthResponseDto> OAuthLoginAsync(OAuthLoginDto dto)
    {
        User? oauthUser = dto.Provider.ToLower() switch
        {
            "google" => await _authService.ValidateGoogleTokenAsync(dto.IdToken),
            "apple" => await _authService.ValidateAppleTokenAsync(dto.IdToken),
            "twitter" => await _authService.ValidateTwitterTokenAsync(dto.IdToken),
            _ => throw new InvalidOperationException("Invalid OAuth provider")
        };

        if (oauthUser == null)
        {
            throw new UnauthorizedAccessException("Invalid OAuth token");
        }

        var existingUser = await _userRepository.GetByEmailAsync(oauthUser.Email);
        
        if (existingUser == null)
        {
            existingUser = await _userRepository.CreateAsync(oauthUser);
        }

        var token = _authService.GenerateJwtToken(existingUser);

        return new AuthResponseDto(
            token,
            MapToUserResponse(existingUser)
        );
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return MapToUserResponse(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToUserResponse);
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;
        
        if (!string.IsNullOrWhiteSpace(dto.Phone))
            user.Phone = dto.Phone;
        
        if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
            user.PhotoUrl = dto.PhotoUrl;

        user = await _userRepository.UpdateAsync(user);

        return MapToUserResponse(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        await _userRepository.DeleteAsync(id);
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!_authService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        user.PasswordHash = _authService.HashPassword(dto.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    private static UserResponseDto MapToUserResponse(User user)
    {
        return new UserResponseDto(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.PhotoUrl,
            user.CreatedAt
        );
    }
}
