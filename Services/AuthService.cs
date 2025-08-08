using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace G3NexusBackend.Services;

public class AuthService : IAuthService
{
    private readonly G3NexusDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(G3NexusDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<ApiResponse> AuthenticateAsync(LoginDTO loginDto)
    {
        try
        {
            // Validate the email and password
            var email = loginDto.EmailAddress;
            var password = loginDto.Password;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new ApiResponse { Status = false, Message = "Email and password are required" };
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email && c.IsActive);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
            if (client == null && employee == null)
            {
                return new ApiResponse { Status = false, Message = "Invalid email or password" };
            }

            var userRole = client?.Role ?? employee?.Role;
            var userId = client?.Id ?? employee?.EmployeeId;

            // Check the password. Hashed password is saved in the database
            var hashedPassword = client?.Password ?? employee?.Password;
            if (!BCrypt.Net.BCrypt.Verify(password, hashedPassword))
            {
                return new ApiResponse { Status = false, Message = "Invalid email or password" };
            }

            var refreshToken = GenerateJwtToken(email, userRole!, userId!.Value.ToString(), TokenType.RefreshToken);
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
                    AccessToken = GenerateJwtToken(email, userRole!, userId!.Value.ToString(), TokenType.AccessToken),
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
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
            var newAccessToken = GenerateJwtToken(userEmail, userRole, userId, TokenType.AccessToken);

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

    public async Task<bool> IsValidEmail(string email)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
        return client != null || employee != null;
    }

    private string GenerateJwtToken(string email, string role, string userId, TokenType tokenType)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, email),
            new (ClaimTypes.NameIdentifier, userId.ToString()),
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

    public async Task CreateVerificationTokenAsync(string email)
    {
        var verificationToken = new Verification
        {
            VerificationCode = new Random().Next(100000, 999999).ToString(),
            Email = email,
            ExpiryDate = DateTime.UtcNow.AddMinutes(5)
        };

        var result = await _context.Verifications.AddAsync(verificationToken);
        if (result.State == EntityState.Added)
        {
            await _context.SaveChangesAsync();
            await _emailService.GetEmailTemplateAsync("VerificationEmailTemplate.html")
                .ContinueWith(async template =>
                {
                    var body = template.Result.Replace("{{VerificationCode}}", verificationToken.VerificationCode);
                    await _emailService.SendEmailAsync(email, "Email Verification", body, true);
                });
        }
        else
        {
            throw new Exception("Failed to create verification token");
        }
    }

    public async Task<bool> IsValidVerificationToken(string email, string verificationCode)
    {
        var verification = await _context.Verifications
            .FirstOrDefaultAsync(v => v.Email == email && v.VerificationCode == verificationCode);

        return verification != null && verification.ExpiryDate >= DateTime.UtcNow;
    }

    public async Task<bool> ChangePasswordAsync(string email, string newPassword)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

        if (client != null)
        {
            client.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Clients.Update(client);
        }
        else if (employee != null)
        {
            employee.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Employees.Update(employee);
        }
        else
        {
            return false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

}