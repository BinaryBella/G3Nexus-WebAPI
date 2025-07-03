using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

public class CompanyService : ICompanyService
{
    private readonly G3NexusDbContext _context;

    public CompanyService(G3NexusDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> GetAllAsync()
    {
        var companies = await _context.Companies
            .Where(c => c.IsActive)
            .Select(c => new CompanyDTO
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                Address = c.Address,
                IsActive = c.IsActive
            }).ToListAsync();

        return new ApiResponse { Status = true, Data = companies };
    }

    public async Task<ApiResponse> GetByIdAsync(int companyId)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null || !company.IsActive)
            return new ApiResponse { Status = false, Message = "Not Found" };

        var dto = new CompanyDTO
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            Address = company.Address,
            IsActive = company.IsActive
        };

        return new ApiResponse { Status = true, Data = dto };
    }

    public async Task<ApiResponse> CreateAsync(CompanyDTO dto)
    {
        var company = new Company
        {
            CompanyName = dto.CompanyName,
            Address = dto.Address,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        dto.CompanyId = company.CompanyId;
        dto.IsActive = true;

        return new ApiResponse { Status = true, Message = "Created", Data = dto };
    }

    public async Task<ApiResponse> UpdateAsync(CompanyDTO dto)
    {
        var company = await _context.Companies.FindAsync(dto.CompanyId);
        if (company == null)
            return new ApiResponse { Status = false, Message = "Not Found" };

        company.CompanyName = dto.CompanyName;
        company.Address = dto.Address;

        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Updated", Data = dto };
    }

    public async Task<ApiResponse> SoftDeleteAsync(int companyId)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null)
            return new ApiResponse { Status = false, Message = "Not Found" };

        company.IsActive = false;
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Deleted" };
    }
}