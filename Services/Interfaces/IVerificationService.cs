using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IVerificationService
    {
        Task<ApiResponse> AddVerification(VerificationDTO verificationDto);
    }
}