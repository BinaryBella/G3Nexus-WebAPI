using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity;

namespace G3NexusBackend.Services
{
    public class UserService : IUserService
    {
        private readonly G3NexusDbContext _context;

        public UserService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                ContactNo = user.ContactNo,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                Password = user.Password,
                Role = user.Role
            }).ToListAsync();
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                ContactNo = user.ContactNo,
                EmailAddress = user.EmailAddress,
                Address = user.Address,
                Password = user.Password,
                Role = user.Role
            };
        }

        public async Task AddUserAsync(UserDTO userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                ContactNo = userDto.ContactNo,
                EmailAddress = userDto.EmailAddress,
                Address = userDto.Address,
                Role = userDto.Role
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateUserAsync(int userId, UserDTO userDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            user.Name = userDto.Name;
            user.ContactNo = userDto.ContactNo;
            user.EmailAddress = userDto.EmailAddress;
            user.Address = userDto.Address;
            user.Role = userDto.Role;
            
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, userDto.Password);

            await _context.SaveChangesAsync();
        }
    }
}
