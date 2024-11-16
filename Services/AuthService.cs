using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using G3NexusBackend.DTOs;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthService
{
    private readonly G3NexusDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(G3NexusDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse> AuthenticateAsync(UserDTO userDto)
    {
        try
        {
            var (isValid, email, role) = await ValidateUserAsync(userDto);
            if (!isValid)
                return new ApiResponse { Status = false, Message = "Invalid credentials" };

            var token = GenerateJwtToken(email, role);
            return new ApiResponse { Status = true, Message = "Authentication successful", Data = token };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Status = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDTO.AccessToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
                return new ApiResponse { Status = false, Message = "Invalid access token" };

            // Retrieve the refresh token
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDTO.RefreshToken && rt.Email == email);

            if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow || refreshToken.IsRevoked ||
                !refreshToken.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Invalid or expired refresh token" };
            }

            // Check if the user exists in Clients or Employees
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

            if (client == null && employee == null)
                return new ApiResponse { Status = false, Message = "User not found" };

            var userEmail = client?.Email ?? employee.Email;
            var userRole = client?.Role ?? employee.Role;

            // Generate new tokens
            var newAccessToken = GenerateJwtToken(userEmail, userRole);
            var newRefreshToken = Guid.NewGuid().ToString();

            // Update the current refresh token to inactive
            refreshToken.IsActive = false;
            refreshToken.IsRevoked = true;

            // Create a new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Email = email,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                Status = true,
                Message = "Tokens refreshed successfully",
                Data = new AuthResponseDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Status = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<bool> VerifyRoleAsync(string email, string requiredRole)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email && c.Role == requiredRole);
        if (client != null)
            return true;

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.Role == requiredRole);
        if (employee != null)
            return true;

        return false;
    }


    private async Task<(bool isValid, string email, string role)> ValidateUserAsync(UserDTO userDto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == userDto.Email);
        if (client != null && BCrypt.Net.BCrypt.Verify(userDto.Password, client.Password))
            return (true, client.Email, client.Role);

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userDto.Email);
        if (employee != null && BCrypt.Net.BCrypt.Verify(userDto.Password, employee.Password))
            return (true, employee.Email, employee.Role);

        return (false, null, null);
    }

    private string GenerateJwtToken(string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add additional claims (optional)
        if (role == "ClientAdmin" || role == "CompanyAdmin")
        {
            claims.Add(new Claim("UserType", "Admin"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            ValidateLifetime = false // Allow expired tokens
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}