using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementController : ControllerBase
    {
        private readonly IRequirementService _requirementService;

        public RequirementController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRequirement([FromBody] RequirementDTO requirementDto)
        {
            var response = await _requirementService.AddRequirement(requirementDto);
            return Ok(response);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditRequirement([FromBody] RequirementDTO requirementDto)
        {
            var response = await _requirementService.EditRequirement(requirementDto);
            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRequirements()
        {
            var response = await _requirementService.GetAllRequirements();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequirementById(int id)
        {
            var response = await _requirementService.GetRequirementById(id);
            return Ok(response);
        }
    }

}