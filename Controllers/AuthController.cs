using G3NexusBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDto)
    {
        if (userDto == null)
        {
            return BadRequest(new ApiResponse { Status = false, Message = "Invalid request" });
        }

        var response = await _authService.AuthenticateAsync(userDto);
        if (response.Status)
        {
            return Ok(response);
        }

        return Unauthorized(response);
    }
}