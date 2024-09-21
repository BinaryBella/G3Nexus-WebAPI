using G3NexusBackend.DTOs;
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

        public async Task<ApiResponse> AddUser(UserDTO userDto)
        {
            // Create a new User entity
            var user = new User
            {
                Name = userDto.Name,
                ContactNo = userDto.ContactNo,
                EmailAddress = userDto.EmailAddress,
                Address = userDto.Address,
                Role = userDto.Role
            };

            // Use PasswordHasher to hash the user's password
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, userDto.Password);

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "User added successfully" };
        }
        
        public async Task<ApiResponse> EditUser(UserDTO userDto)
        {
            var user = await _context.Users.FindAsync(userDto.UserId);
    
            if (user == null)
            {
                return new ApiResponse { Status = false, Message = "User not found" };
            }

            // Update other fields
            user.Name = userDto.Name;
            user.ContactNo = userDto.ContactNo;
            user.EmailAddress = userDto.EmailAddress;
            user.Address = userDto.Address;

            // Hash the password only if the user is trying to update the password
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, userDto.Password);
            }

            // Update the role
            user.Role = userDto.Role;

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "User updated successfully" };
        }

        public async Task<ApiResponse> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return new ApiResponse { Status = true, Data = users };
        }

        public async Task<ApiResponse> GetUserById(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse { Status = false, Message = "User not found" };
            }

            return new ApiResponse { Status = true, Data = user };
        }
    }
}