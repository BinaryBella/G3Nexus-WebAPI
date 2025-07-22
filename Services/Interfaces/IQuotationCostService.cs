using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;

namespace G3NexusBackend.Services.Interfaces;

public interface IQuotationCostService
{
    Task<QuotationCost> SaveQuotationCostAsync(QuotationCostDTO dto);
    Task<QuotationCost?> GetByProjectIdAsync(int projectId);
}