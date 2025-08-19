using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces
{
    public interface IQuotationService
    {
        Task<List<QuotationDisplayDTO>> GetAllQuotationsAsync();
        Task<QuotationDetailDTO?> GetQuotationByIdAsync(int quotationId);
        Task<List<QuotationDisplayDTO>> GetQuotationsByClientIdAsync(int clientId);
        Task<List<QuotationDisplayDTO>> GetQuotationsByProjectIdAsync(int projectId);
        Task<List<QuotationDisplayDTO>> GetQuotationsByEmployeeIdAsync(int employeeId);
        Task<List<QuotationDisplayDTO>> GetQuotationsByTypeAsync(string type);
        Task<List<QuotationDisplayDTO>> GetQuotationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<QuotationSummaryDTO> GetQuotationSummaryAsync();
    }
}
