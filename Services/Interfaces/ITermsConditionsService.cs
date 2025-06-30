using G3NexusBackend.DTOs;

namespace G3NexusBackend.Services.Interfaces;

public interface ITermsConditionsService
{
    Task<IEnumerable<TermsConditionsDTO>> GetAllTermsAsync();
    Task<TermsConditionsDTO?> GetTermsByIdAsync(int TCId);
    Task<TermsConditionsDTO> CreateTermsAsync(TermsConditionsDTO bugDto);
}