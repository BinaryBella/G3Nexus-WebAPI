using G3NexusBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> AddUser(UserDTO userDto)
    {
        var response = await _userService.AddUser(userDto);
        return Ok(response);
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditUser(UserDTO userDto)
    {
        var response = await _userService.EditUser(userDto);
        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsers();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var response = await _userService.GetUserById(id);
        return Ok(response);
    }
}

