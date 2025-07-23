using G3NexusBackend.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Services.Interfaces;

public interface IBugService
{ 
    Task<IEnumerable<BugDTO>> GetAllBugsAsync(int userId, DateTime userLastLogin);
    Task<BugDTO?> GetBugByIdAsync(int bugId);
    Task<BugDTO> CreateBugAsync(BugDTO bugDto);
    Task<BugDTO?> UpdateBugAsync(int bugId, BugDTO bugDto);
    Task<ApiResponse> DeActivateBugAsync(int bugId);
    Task<BugDTO?> MarkAsViewedAsync(int requirementId);
}