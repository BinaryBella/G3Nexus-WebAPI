using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task AddUserAsync(UserDTO userDto);
        Task UpdateUserAsync(int userId, UserDTO userDto);
    }
}