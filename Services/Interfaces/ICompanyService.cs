using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync();
    Task<CompanyDTO?> GetCompanyByIdAsync(int CompanyId);
    Task<CompanyDTO> CreateCompaniesAsync(CompanyDTO companyDto);
    Task<CompanyDTO?> UpdateCompaniesAsync(CompanyDTO companyDto);
    Task<ApiResponse> DeActivateCompanyAsync(int CompanyId);
}