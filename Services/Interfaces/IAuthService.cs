using G3NexusBackend.DTOs;

public interface IAuthService
{
    Task<ApiResponse> AuthenticateAsync(UserDTO userDto);
}