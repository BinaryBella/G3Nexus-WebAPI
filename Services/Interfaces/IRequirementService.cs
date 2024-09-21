using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IRequirementService
    {
        Task<ApiResponse> AddRequirement(RequirementDTO requirementDto);
        Task<ApiResponse> EditRequirement(RequirementDTO requirementDto);
        Task<ApiResponse> GetAllRequirements();
        Task<ApiResponse> GetRequirementById(int requirementId);
    }
}