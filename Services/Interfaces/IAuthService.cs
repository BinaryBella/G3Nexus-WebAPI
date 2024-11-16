using G3NexusBackend.DTOs;

public interface IAuthService
{
    Task<ApiResponse> AuthenticateAsync(UserDTO userDto);
    Task<ApiResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO);
    Task<bool> VerifyRoleAsync(string email, string requiredRole);
}