using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using G3NexusBackend.DTOs;
using G3NexusBackend.Models;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly G3NexusDbContext _dbContext; // Replace with your actual DbContext

    public AuthService(IConfiguration config, G3NexusDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }
    
    public User? IsAuthenticated(string emailAddress, string password)
    {
        // Assuming you're fetching the user from the database based on email and password
        var user = _dbContext.Users.FirstOrDefault(u => u.EmailAddress == emailAddress && u.Password == password);

        if (user == null)
        {
            return null; // User not found or incorrect password
        }

        return user;
    }
    
}