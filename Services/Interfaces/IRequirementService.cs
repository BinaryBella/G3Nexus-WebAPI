using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IRequirementService
{
    Task<IEnumerable<RequirementListItemDTO>> GetAllRequirementsAsync();
    Task<IEnumerable<RequirementListItemDTO>> GetRequirementsByProjectAsync(RequirementsByProjectRequestDTO request);
    Task<RequirementDTO?> GetRequirementByIdAsync(int requirementId);
    Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto);
    Task<RequirementDTO?> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto);
    Task<ApiResponse> DeActivateRequirementAsync(int requirementId);
    Task<ApiResponse> SendRequirementQuotationAsync(RequirementQuotationRequestDTO quotationRequest);
    Task<ApiResponse> SendBulkQuotationAsync(BulkQuotationRequestDTO bulkQuotationRequest);
    Task<ApiResponse> ValidateBulkQuotationSelectionAsync(BulkQuotationValidationDTO validation);
}