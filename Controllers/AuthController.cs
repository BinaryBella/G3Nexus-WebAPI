using System.Security.Claims;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace G3NexusBackend.Controllers;

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
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var response = await _authService.AuthenticateAsync(loginDto);
        if (response.Status)
        {
            return Ok(response);
        }

        return Unauthorized(response);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
    {
        var authResponse = await _authService.RefreshTokenAsync(refreshTokenDto);
        return Ok(authResponse);
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new { message = "Invalid user" });
        }

        var result = await _authService.LogoutAsync(userEmail);
        return result ? Ok(new { message = "Logged out successfully" }) : BadRequest(new { message = "Logout failed" });
    }
  

    
}