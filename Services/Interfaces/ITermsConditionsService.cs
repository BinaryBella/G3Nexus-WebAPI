using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces;

public interface ITermsConditionsService
{
    Task<ApiResponse> GetAllAsync();
    Task<ApiResponse> CreateAsync(TermsConditionsDTO dto);
    Task<ApiResponse> UpdateAsync(TermsConditionsDTO dto);
}