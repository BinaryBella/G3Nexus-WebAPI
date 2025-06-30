using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IRequirementService
{
    Task<IEnumerable<RequirementDTO>> GetAllRequirementsAsync();
    Task<RequirementDTO?> GetRequirementByIdAsync(int requirementId);
    Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto);
    Task<RequirementDTO?> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto);
    Task<ApiResponse> DeActivateRequirementAsync(int requirementId);
}