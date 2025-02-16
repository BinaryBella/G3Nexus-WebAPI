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

    public async Task<ApiResponse> AuthenticateAsync(LoginDTO loginDto)
    {
        try
        {
            var (isValid, email, role) = await ValidateUserAsync(loginDto);
            if (!isValid)
            {
                return new ApiResponse { Status = false, Message = "Invalid credentials" };
            }
            
            var refreshToken = GenerateJwtToken(email, role, TokenType.RefreshToken);
            var refreshTokenObject = new RefreshToken
            {
                Email = email,
                Token = refreshToken,
                IsActive = true,
                ExpiryDate = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"]))
                
            };
            await _context.RefreshTokens.AddAsync(refreshTokenObject);
            await _context.SaveChangesAsync();
            return new ApiResponse { Status = true, Message = "Authentication successful", Data =
            new {
                AccessToken = GenerateJwtToken(email, role, TokenType.AccessToken),
                RefreshToken = refreshToken
            } };
            
        }
        catch (Exception ex)
        {
            return new ApiResponse { Status = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
                return new ApiResponse { Status = false, Message = "Invalid access token" };

            // Retrieve the refresh token
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken && rt.Email == email);

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

            var userEmail = client?.Email ?? employee?.Email;
            var userRole = client?.Role ?? employee?.Role;
            
            if (userEmail == null || userRole == null)
                return new ApiResponse { Status = false, Message = "User not found" };

            // Generate new tokens
            var newAccessToken = GenerateJwtToken(userEmail, userRole, TokenType.AccessToken);

            return new ApiResponse
            {
                Status = true,
                Message = "Tokens refreshed successfully",
                Data = new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = refreshTokenDto.RefreshToken
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Status = false, Message = $"An error occurred: {ex.Message}" };
        }
    }

    private async Task<(bool isValid, string email, string role)> ValidateUserAsync(LoginDTO loginDto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == loginDto.EmailAddress);
        if (client != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, client.Password))
            return (true, client.Email, client.Role);

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == loginDto.EmailAddress);
        if (employee != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, employee.Password))
            return (true, employee.Email, employee.Role);

        return (false, null, null);
    }

    private string GenerateJwtToken(string email, string role, TokenType tokenType)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, email),
            new (ClaimTypes.Role, role),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add additional claims (optional)
        if (role is "ClientAdmin" or "CompanyAdmin")
        {
            claims.Add(new Claim("UserType", "Admin"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"]));
        if (tokenType == TokenType.RefreshToken)
        {
            expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"]));
        }

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
    
    public async Task<bool> LogoutAsync(string email)
    {
        var refreshToken = await _context.RefreshTokens
            .Where(rt => rt.Email == email)
            .FirstOrDefaultAsync();

        if (refreshToken == null)
        {
            return false; 
        }

        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();

        return true; // Logout successful
    }

}