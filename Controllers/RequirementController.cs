using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
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

        // GET: api/Requirement?userId=12&lastLogin=2025-07-23T06:00:00Z
        [HttpGet]
        public async Task<IActionResult> GetRequirements([FromQuery] int userId, [FromQuery] DateTime lastLogin)
        {
            var requirements = await _requirementService.GetAllRequirementsAsync(userId, lastLogin);
            return Ok(new ApiResponse { Status = true, Data = requirements });
        }

        [HttpGet("{id:int}")]
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
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
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
