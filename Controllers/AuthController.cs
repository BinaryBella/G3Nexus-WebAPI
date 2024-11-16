using System.Security.Claims;
using G3NexusBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
    {
        var authResponse = await _authService.RefreshTokenAsync(refreshTokenDTO);
        return Ok(authResponse);
    }

    [Authorize]
    [HttpGet("verify-role")]
    public async Task<IActionResult> VerifyRole([FromQuery] string requiredRole)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null) return Unauthorized();

        var isAuthorized = await _authService.VerifyRoleAsync(email, requiredRole);
        return isAuthorized ? Ok("Role verified.") : Forbid();
    }
}