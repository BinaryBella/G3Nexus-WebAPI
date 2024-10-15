using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using G3NexusBackend.DTOs;
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
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == userDto.Email);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userDto.Email);

        if (client == null && employee == null)
        {
            return new ApiResponse { Status = false, Message = "User not found" };
        }

        // Check if user exists and verify password
        if (client != null && BCrypt.Net.BCrypt.Verify(userDto.Password, client.Password))
        {
            var token = GenerateJwtToken(client.Email, client.Role);
            return new ApiResponse { Status = true, Message = "Authentication successful", Data = token };
        }
        else if (employee != null && BCrypt.Net.BCrypt.Verify(userDto.Password, employee.Password))
        {
            var token = GenerateJwtToken(employee.Email, employee.Role);
            return new ApiResponse { Status = true, Message = "Authentication successful", Data = token };
        }

        return new ApiResponse { Status = false, Message = "Invalid credentials" };
    }

    private string GenerateJwtToken(string email, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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
}
