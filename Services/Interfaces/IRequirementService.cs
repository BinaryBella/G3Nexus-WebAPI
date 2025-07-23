using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IRequirementService
{
    Task<IEnumerable<RequirementDTO>> GetAllRequirementsAsync(int userId, DateTime userLastLogin);
    Task<RequirementDTO?> GetRequirementByIdAsync(int requirementId);
    Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto);
    Task<RequirementDTO?> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto);
    Task<ApiResponse> DeActivateRequirementAsync(int requirementId);
    Task<RequirementDTO?> MarkAsViewedAsync(int requirementId);
}