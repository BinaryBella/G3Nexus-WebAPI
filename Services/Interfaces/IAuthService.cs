using G3NexusBackend.Data.DTO;
using G3NexusBackend.DTOs;
using G3NexusBackend.Models;

public interface IAuthService
{
    User? IsAuthenticated(string emailaddress, string password);
    bool DoesEmailExists(string email);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDTO model);
    Task<ApiResponse> VerifyCodeAsync(VerifyCodeDTO model);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO model);
}