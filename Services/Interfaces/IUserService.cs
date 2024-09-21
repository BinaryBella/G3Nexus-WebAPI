using G3NexusBackend.DTOs;

public interface IUserService
{
    Task<ApiResponse> AddUser(UserDTO userDto);
    Task<ApiResponse> EditUser(UserDTO userDto);
    Task<ApiResponse> GetAllUsers();
    Task<ApiResponse> GetUserById(int userId);
}