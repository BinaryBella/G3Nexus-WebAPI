using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse> AuthenticateAsync(LoginDTO loginDto);
    Task<ApiResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto);
    Task<bool> LogoutAsync(string email);
}