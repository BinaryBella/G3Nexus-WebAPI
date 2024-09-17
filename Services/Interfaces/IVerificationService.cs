using G3NexusBackend.DTOs;
using System.Threading.Tasks;

namespace G3NexusBackend.Interfaces
{
    public interface IVerificationService
    {
        Task AddVerificationAsync(VerificationDTO verificationDto);
    }
}