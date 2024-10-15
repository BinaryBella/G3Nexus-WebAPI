using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetRequirements()
        {
            var requirements = await _requirementService.GetAllRequirementsAsync();
            return Ok(new ApiResponse { Status = true, Data = requirements });
        }

        [HttpGet("{id}")]
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
        public async Task<IActionResult> CreateRequirement(RequirementDTO requirementDto)
        {
            var requirement = await _requirementService.CreateRequirementAsync(requirementDto);
            return CreatedAtAction(nameof(GetRequirementById), new { id = requirement.RequirementId }, new ApiResponse { Status = true, Data = requirement });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequirement(int id, RequirementDTO requirementDto)
        {
            var requirement = await _requirementService.UpdateRequirementAsync(id, requirementDto);
            if (requirement == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Requirement not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = requirement });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateRequirement(int id)
        {
            var response = await _requirementService.DeActivateRequirementAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
