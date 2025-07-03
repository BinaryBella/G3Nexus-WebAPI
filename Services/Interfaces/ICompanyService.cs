using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface ICompanyService
{
    Task<ApiResponse> GetAllAsync();
    Task<ApiResponse> GetByIdAsync(int companyId);
    Task<ApiResponse> CreateAsync(CompanyDTO dto);
    Task<ApiResponse> UpdateAsync(CompanyDTO dto);
    Task<ApiResponse> SoftDeleteAsync(int companyId);
}