using G3NexusBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Services.Interfaces;

public interface IVerificationService
{
    Task<VerificationDTO> GenerateVerificationCode(string email);
    Task<bool> ValidateVerificationCode(ValidateVerificationCodeRequest request);
    Task<bool> ValidateVerificationCode(string email, string code);
}
