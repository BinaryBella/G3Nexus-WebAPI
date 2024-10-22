using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IRequirementService
    {
        Task<IEnumerable<RequirementDTO>> GetAllRequirementsAsync();
        Task<RequirementDTO> GetRequirementByIdAsync(int requirementId);
        Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto);
        Task<RequirementDTO> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto);
        Task<ApiResponse> DeActivateRequirementAsync(int requirementId);
    }
}  