using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly IRequirementService _requirementService;

        public RequirementsController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementDTO>>> GetAllRequirements()
        {
            var requirements = await _requirementService.GetAllRequirementsAsync();
            return Ok(requirements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementDTO>> GetRequirementById(int id)
        {
            var requirement = await _requirementService.GetRequirementByIdAsync(id);
            if (requirement == null) return NotFound();

            return Ok(requirement);
        }

        [HttpPost]
        public async Task<ActionResult> AddRequirement(RequirementDTO requirementDto)
        {
            await _requirementService.AddRequirementAsync(requirementDto);
            return CreatedAtAction(nameof(GetRequirementById), new { id = requirementDto.RequirementId }, requirementDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRequirement(int id, RequirementDTO requirementDto)
        {
            await _requirementService.UpdateRequirementAsync(id, requirementDto);
            return NoContent();
        }
    }
}