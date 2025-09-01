using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly IRequirementService _requirementService;

        public RequirementController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        // GET: api/Requirement?userId=12&lastLogin=2025-07-23T06:00:00Z
    [HttpGet]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetRequirements()
        {
            var requirements = await _requirementService.GetAllRequirementsAsync();
            return Ok(new ApiResponse { Status = true, Data = requirements });
        }

    [HttpPost("by-project")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetRequirementsByProject(RequirementsByProjectRequestDTO request)
        {
            var requirements = await _requirementService.GetRequirementsByProjectAsync(request);
            return Ok(new ApiResponse { Status = true, Data = requirements });
        }

    [HttpPost("validate-bulk-selection")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> ValidateBulkQuotationSelection(BulkQuotationValidationDTO validation)
        {
            var response = await _requirementService.ValidateBulkQuotationSelectionAsync(validation);
            return Ok(response);
        }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetRequirementById(int id)
        {
            var requirement = await _requirementService.GetRequirementByIdAsync(id);
            if (requirement == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Requirement not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = requirement });
        }

    [HttpPost]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER")]
    public async Task<IActionResult> CreateRequirement(RequirementDTO requirementDto)
        {
            try
            {
                var requirement = await _requirementService.CreateRequirementAsync(requirementDto);
                return CreatedAtAction(nameof(GetRequirementById), new { id = requirement.RequirementId }, new ApiResponse { Status = true, Data = requirement });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

    [HttpPut]
    [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> UpdateRequirement(RequirementDTO requirementDto)
        {
            try
            {
                var id = requirementDto.RequirementId;
                var requirement = await _requirementService.UpdateRequirementAsync(id, requirementDto);
                if (requirement == null)
                {
                    return NotFound(new ApiResponse { Status = false, Message = "Requirement not found" });
                }

                return Ok(new ApiResponse { Status = true, Data = requirement });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> UpdateRequirementStatus(int id, [FromBody] StatusUpdateDTO statusUpdate)
        {
            var response = await _requirementService.UpdateRequirementStatusAsync(id, statusUpdate.Status);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> DeactivateRequirement(int id)
        {
            var response = await _requirementService.DeActivateRequirementAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

    [HttpPost("send-quotation")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> SendRequirementQuotation(RequirementQuotationRequestDTO quotationRequest)
        {
            try
            {
                var response = await _requirementService.SendRequirementQuotationAsync(quotationRequest);
                if (!response.Status)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

    [HttpPost("send-bulk-quotation")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> SendBulkQuotation(BulkQuotationRequestDTO bulkQuotationRequest)
        {
            try
            {
                var response = await _requirementService.SendBulkQuotationAsync(bulkQuotationRequest);
                if (!response.Status)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }
    }
}
