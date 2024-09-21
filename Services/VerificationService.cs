using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;

namespace G3NexusBackend.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly G3NexusDbContext _context;

        public VerificationService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> AddVerification(VerificationDTO verificationDto)
        {
            var verification = new Verification
            {
                UserId = verificationDto.UserId,
                VerificationCode = verificationDto.VerificationCode,
                ExpiryDate = verificationDto.ExpiryDate
            };

            _context.Verifications.Add(verification);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Verification added successfully" };
        }
    }
}