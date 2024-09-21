using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProject([FromBody] ProjectDTO projectDto)
        {
            var response = await _projectService.AddProject(projectDto);
            return Ok(response);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditProject([FromBody] ProjectDTO projectDto)
        {
            var response = await _projectService.EditProject(projectDto);
            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects()
        {
            var response = await _projectService.GetAllProjects();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var response = await _projectService.GetProjectById(id);
            return Ok(response);
        }
    }

}