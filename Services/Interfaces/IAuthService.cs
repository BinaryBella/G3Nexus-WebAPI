using G3NexusBackend.DTOs;

public interface IAuthService
{
    Task<ApiResponse> AuthenticateAsync(LoginDTO loginDto);
    Task<ApiResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto);
    Task<bool> LogoutAsync(string email);
}