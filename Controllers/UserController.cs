using G3NexusBackend.Models;
using G3NexusBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser(User user)
        {
            if (user == null)
            {
                return BadRequest("User is null.");
            }

            // Validate the user object (you can use ModelState for validation if needed)
            var createdUser = await _userService.AddUserAsync(user);
            return Ok(createdUser);
        }

        [HttpPut("edit/{userId}")]
        public async Task<IActionResult> EditUser(int userId, User user)
        {
            if (user == null)
            {
                return BadRequest("User is null.");
            }

            var updatedUser = await _userService.UpdateUserAsync(userId, user);

            if (updatedUser == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return Ok(updatedUser);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            return Ok(user);
        }
    }
}
