using G3NexusBackend.Models;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(int userId, User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);

    }
}
