using G3NexusBackend.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Services.Interfaces;

public interface IBugService
{
    Task<IEnumerable<BugListItemDTO>> GetAllBugsAsync(int userId);
    Task<BugDTO?> GetBugByIdAsync(int bugId);
    Task<BugDTO> CreateBugAsync(BugDTO bugDto);
    Task<BugDTO?> UpdateBugAsync(int bugId, BugDTO bugDto);
    Task<ApiResponse> DeActivateBugAsync(int bugId);
    Task<ApiResponse> SendBugQuotationAsync(BugQuotationRequestDTO quotationRequest);
    Task<ApiResponse> SendBulkBugQuotationAsync(BulkBugQuotationRequestDTO bulkQuotationRequest);
}