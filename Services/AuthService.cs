using System.Security.Cryptography;
using G3NexusBackend.Models;
using System.Text;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Interfaces;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly G3NexusDbContext _dbContext;
    private readonly IEmailService _emailService;

    public AuthService(IConfiguration config, G3NexusDbContext dbContext, IEmailService emailService)
    {
        _config = config;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    // Authenticate by checking the email and hashed password
    public User? IsAuthenticated(string emailAddress, string password)
    {
        // Hash the password entered by the user
        var hashedPassword = HashPassword(password);

        // Find the user by email (case-insensitive) and compare hashed passwords
        var user = _dbContext.Users
            .FirstOrDefault(u => u.EmailAddress.ToLower() == emailAddress.ToLower() && u.Password == hashedPassword);

        if (user == null)
        {
            return null;
        }

        return user;
    }

    public bool DoesEmailExists(string email)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.EmailAddress == email);
        return user != null;
    }

    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDTO model)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == model.Email);
        if (user == null)
        {
            return new ApiResponse { Status = false, Message = "User not found." };
        }

        var verificationCode = GenerateVerificationCode();
        var verification = new Verification
        {
            UserId = user.UserId,
            VerificationCode = verificationCode,
            ExpiryDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _dbContext.Verifications.AddAsync(verification);
        await _dbContext.SaveChangesAsync();

        // Send verification code via email
        await _emailService.SendEmailAsync(user.EmailAddress, "Password Reset Verification Code", $"Your code is {verificationCode}");

        return new ApiResponse { Status = true, Message = "Verification code sent to email." };
    }

    public async Task<ApiResponse> VerifyCodeAsync(VerifyCodeDTO model)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == model.Email);
        if (user == null)
        {
            return new ApiResponse { Status = false, Message = "User not found." };
        }

        var verification = await _dbContext.Verifications
            .FirstOrDefaultAsync(x => x.UserId == user.UserId && x.VerificationCode == model.VerificationCode && x.ExpiryDate > DateTime.UtcNow);

        if (verification == null)
        {
            return new ApiResponse { Status = false, Message = "Invalid or expired verification code." };
        }

        return new ApiResponse { Status = true, Message = "Verification successful." };
    }

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO model)
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            return new ApiResponse { Status = false, Message = "Passwords do not match." };
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == model.Email);
        if (user == null)
        {
            return new ApiResponse { Status = false, Message = "User not found." };
        }

        user.Password = HashPassword(model.NewPassword); // Using the BCrypt HashPassword method
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Password reset successfully." };
    }

    private string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private string HashPassword(string password)
    {
        // BCrypt for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
