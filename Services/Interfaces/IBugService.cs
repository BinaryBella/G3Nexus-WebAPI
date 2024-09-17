using G3NexusBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Interfaces
{
    public interface IBugService
    {
        Task<IEnumerable<BugDTO>> GetAllBugsAsync();
        Task<BugDTO> GetBugByIdAsync(int bugId);
        Task AddBugAsync(BugDTO bugDto);
        Task UpdateBugAsync(int bugId, BugDTO bugDto);
    }
}