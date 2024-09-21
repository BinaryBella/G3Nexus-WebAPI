using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IBugService
    {
        Task<ApiResponse> AddBug(BugDTO bugDto);
        Task<ApiResponse> EditBug(BugDTO bugDto);
        Task<ApiResponse> GetAllBugs();
        Task<ApiResponse> GetBugById(int bugId);
    }
}