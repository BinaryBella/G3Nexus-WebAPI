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
  
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        var isValidEmail = await _authService.IsValidEmail(email);
        if (!isValidEmail)
        {
            return NotFound(new { message = "Email not found" });
        }

        await _authService.CreateVerificationTokenAsync(email);
        return Ok(new { message = "Verification Code has been set to your email" });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] ResetPasswordVerificationDTO data)
    {
        if (string.IsNullOrEmpty(data.VerificationCode) || string.IsNullOrEmpty(data.Email))
        {
            return BadRequest(new { message = "Verification code and email are required" });
        }

        var isValid = await _authService.IsValidVerificationToken(data.Email, data.VerificationCode);
        if (isValid)
        {
            return Ok(new { message = "Email verified successfully" });
        }

        return BadRequest(new { message = "Invalid verification code or email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
    {
        if (string.IsNullOrEmpty(resetPasswordDto.VerificationCode))
        {
            return BadRequest(new { message = "Verification code is required" });
        }

        if (string.IsNullOrEmpty(resetPasswordDto.EmailAddress) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
        {
            return BadRequest(new { message = "Email and new password are required" });
        }

        var isValidVerificationToken = await _authService.IsValidVerificationToken(resetPasswordDto.EmailAddress, resetPasswordDto.VerificationCode);
        if (!isValidVerificationToken)
        {
            return BadRequest(new { message = "Invalid verification code or email" });
        }

        var isValidEmail = await _authService.IsValidEmail(resetPasswordDto.EmailAddress);
        if (!isValidEmail)
        {
            return NotFound(new { message = "Email not found" });
        }

        await _authService.ChangePasswordAsync(resetPasswordDto.EmailAddress, resetPasswordDto.NewPassword);

        return Ok(new { message = "Password reset successfully" });
    }
    
}