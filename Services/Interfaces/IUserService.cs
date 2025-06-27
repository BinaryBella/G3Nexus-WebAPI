using G3NexusBackend.DTOs;

namespace G3NexusBackend.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse> AddUser(LoginDTO loginDto);
    Task<ApiResponse> EditUser(LoginDTO loginDto);
    Task<ApiResponse> GetAllUsers();
    Task<ApiResponse> GetUserById(int userId);
}