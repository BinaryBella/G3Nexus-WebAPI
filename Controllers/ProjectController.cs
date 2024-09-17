using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult> AddProject(ProjectDTO projectDto)
        {
            await _projectService.AddProjectAsync(projectDto);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectDto.ProjectId }, projectDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, ProjectDTO projectDto)
        {
            await _projectService.UpdateProjectAsync(id, projectDto);
            return NoContent();
        }
    }
}