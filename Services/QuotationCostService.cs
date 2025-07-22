using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Data;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;

public class QuotationCostService : IQuotationCostService
{
    private readonly G3NexusDbContext _context;

    public QuotationCostService(G3NexusDbContext context)
    {
        _context = context;
    }

    public async Task<QuotationCost> SaveQuotationCostAsync(QuotationCostDTO dto)
    {
        // Check if already exists (update case)
        var existing = await _context.QuotationCosts
            .FirstOrDefaultAsync(q => q.ProjectId == dto.ProjectId);

        if (existing != null)
        {
            existing.AdvancePayment = dto.AdvancePayment;
            existing.DevelopmentCost = dto.DevelopmentCost;
            existing.HostingAndDomain = dto.HostingAndDomain;
            existing.SSLCertificate = dto.SSLCertificate;
            existing.DeploymentCost = dto.DeploymentCost;
        }
        else
        {
            existing = new QuotationCost
            {
                ProjectId = dto.ProjectId,
                AdvancePayment = dto.AdvancePayment,
                DevelopmentCost = dto.DevelopmentCost,
                HostingAndDomain = dto.HostingAndDomain,
                SSLCertificate = dto.SSLCertificate,
                DeploymentCost = dto.DeploymentCost
            };
            _context.QuotationCosts.Add(existing);
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<QuotationCost?> GetByProjectIdAsync(int projectId)
    {
        return await _context.QuotationCosts
            .FirstOrDefaultAsync(q => q.ProjectId == projectId);
    }
}