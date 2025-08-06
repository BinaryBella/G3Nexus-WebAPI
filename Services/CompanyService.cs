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

    public async Task<IEnumerable<CompanyDTO>> GetAllCompaniesAsync()
    {
        return await _context.Companies
            .Where(c => c.IsActive)
            .Select(c => new CompanyDTO
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                Address = c.Address,
                IsActive = c.IsActive
            }).ToListAsync();
    }

public async Task<CompanyDTO?> GetCompanyByIdAsync(int CompanyId)
    {
        var company = await _context.Companies.FindAsync(CompanyId);
        if (company is not {IsActive: true})
        {
            return null;
        }
        
        return new CompanyDTO
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            Address = company.Address,
            IsActive = company.IsActive
        };
    }

    public async Task<ApiResponse> CreateCompaniesAsync(CompanyDTO companyDto)
    {
        // Check if a company with the same name already exists
        var companyExists = await _context.Companies.AnyAsync(c => c.CompanyName == companyDto.CompanyName && c.IsActive);
        if (companyExists)
        {
            return new ApiResponse
            {
                Status = false,
                Message = $"A company with the name '{companyDto.CompanyName}' already exists."
            };
        }

        var company = new Company
        {
            CompanyName = companyDto.CompanyName,
            Address = companyDto.Address,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        companyDto.CompanyId = company.CompanyId;
        return new ApiResponse
        {
            Status = true,
            Data = companyDto,
            Message = "Company created successfully."
        };
    }

    public async Task<CompanyDTO?> UpdateCompaniesAsync(CompanyDTO companyDto)
    {
        var company = await _context.Companies.FindAsync(companyDto.CompanyId);
        if (company is not { IsActive: true })
        {
            return null;
        }

        company.CompanyName = companyDto.CompanyName;
        company.Address = companyDto.Address;

        _context.Companies.Update(company);
        await _context.SaveChangesAsync();

        return companyDto;
    }

    public async Task<ApiResponse> DeActivateCompanyAsync(int CompanyId)
    {
        var company = await _context.Companies
            .Include(c => c.Clients) // Include clients in the query
            .FirstOrDefaultAsync(c => c.CompanyId == CompanyId);

        if (company is not { IsActive: true })
        {
            return new ApiResponse { Status = false, Message = "Company not found or already inactive." };
        }

        var activeCompanyClients = await _context.Clients
            .Where(c => c.CompanyId == CompanyId && c.IsActive)
            .ToListAsync();

        if (activeCompanyClients.Any())
        {
            return new ApiResponse { Status = false, Message = "This company has associated clients and cannot be deleted. Please remove all clients before deleting the company." };
        }

        company.IsActive = false;
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Company successfully deactivated." };
    }
}