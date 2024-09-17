using G3NexusBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Interfaces
{
    public interface IRequirementService
    {
        Task<IEnumerable<RequirementDTO>> GetAllRequirementsAsync();
        Task<RequirementDTO> GetRequirementByIdAsync(int requirementId);
        Task AddRequirementAsync(RequirementDTO requirementDto);
        Task UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto);
    }
}