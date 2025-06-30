using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IBugService
{
    Task<IEnumerable<BugDTO>> GetAllBugsAsync();
    Task<BugDTO?> GetBugByIdAsync(int bugId);
    Task<BugDTO> CreateBugAsync(BugDTO bugDto);
    Task<BugDTO?> UpdateBugAsync(int bugId, BugDTO bugDto);
    Task<ApiResponse> DeActivateBugAsync(int bugId);
}