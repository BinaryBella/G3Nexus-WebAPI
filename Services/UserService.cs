using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public class UserService : IUserService
    {
        private readonly G3NexusDbContext _dbContext;

        public UserService(G3NexusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddUserAsync(User user)
        {
            // Hash the password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Add the user to the database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existingUser == null)
            {
                return null; // User not found
            }

            // Update the properties
            existingUser.Name = user.Name;
            existingUser.ContactNo = user.ContactNo;
            existingUser.EmailAddress = user.EmailAddress;
            existingUser.Address = user.Address;

            // Optionally update the password if it's provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            existingUser.Role = user.Role;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            return existingUser;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
