using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using G3NexusBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly G3NexusDbContext _context;

    public AuthController(IAuthService authService, IJwtService jwtService, G3NexusDbContext context)
    {
        _context = context;
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        // Fetch the user by email (case-insensitive)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress.ToLower() == loginDTO.EmailAddress.ToLower());

        if (user == null)
        {
            return Unauthorized(new ApiResponse
            {
                Status = false,
                Message = "Unauthorized",
                Error = "Invalid credentials",
                Data = null
            });
        }

        // Use PasswordHasher to verify the provided password
        var passwordHasher = new PasswordHasher<User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);

        // Check if the password is correct
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new ApiResponse
            {
                Status = false,
                Message = "Unauthorized",
                Error = "Invalid credentials",
                Data = null
            });
        }

        // If the credentials are valid, generate JWT token and refresh token
        var token = _jwtService.GenerateAccessToken(user.EmailAddress, new[] { user.Role });
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Optionally, save the refresh token in the database
        await _jwtService.AddRefreshTokenAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.UserId,
            ExpiryDate = DateTime.UtcNow.AddDays(7) // Example expiration for refresh token
        });

        return Ok(new ApiResponse
        {
            Status = true,
            Message = "Login successful",
            Data = new
            {
                AccessToken = token,
                RefreshToken = refreshToken
            }
        });
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
    {
        // Validate the refresh token and access token
        var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenDTO.AccessToken);

        var email = principal?.Identity?.Name;
        if (email == null)
        {
            return BadRequest(new ApiResponse
            {
                Status = false,
                Message = "Invalid token",
                Error = "Access token is invalid",
                Data = null
            });
        }

        // Find the refresh token in the database
        var storedRefreshToken = await _jwtService.GetRefreshTokenData(refreshTokenDTO.RefreshToken);
        if (storedRefreshToken == null || storedRefreshToken.IsRevoked ||
            storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(new ApiResponse
            {
                Status = false,
                Message = "Invalid refresh token",
                Error = "Refresh token is either invalid, expired, or revoked",
                Data = null
            });
        }

        // Generate a new access token and refresh token
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == email);
        if (user == null)
        {
            return Unauthorized(new ApiResponse
            {
                Status = false,
                Message = "Unauthorized",
                Error = "User not found",
                Data = null
            });
        }

        var roles = new[] { user.Role }; // Assuming the user has one role
        var newAccessToken = _jwtService.GenerateAccessToken(user.EmailAddress, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke the old refresh token and add the new one
        storedRefreshToken.IsRevoked = true;
        await _jwtService.AddRefreshTokenAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.UserId,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Set a new expiry for the refresh token
            IsRevoked = false
        });

        await _context.SaveChangesAsync();

        // Return the new tokens
        return Ok(new ApiResponse
        {
            Status = true,
            Message = "Token refreshed successfully",
            Data = new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO model)
    {
        var response = await _authService.ForgotPasswordAsync(model);
        if (!response.Status)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDTO model)
    {
        var response = await _authService.VerifyCodeAsync(model);
        if (!response.Status)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        var response = await _authService.ResetPasswordAsync(model);
        if (!response.Status)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}