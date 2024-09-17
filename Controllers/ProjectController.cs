using G3NexusBackend.Models;
using G3NexusBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> AddProject(Project project)
        {
            if (project == null)
            {
                return BadRequest("Project data is null.");
            }

            var createdProject = await _projectService.AddProjectAsync(project);
            return Ok(createdProject);
        }

        [HttpPut("edit/{projectId}")]
        public async Task<IActionResult> EditProject(int projectId, Project project)
        {
            if (project == null)
            {
                return BadRequest("Project data is null.");
            }

            var updatedProject = await _projectService.UpdateProjectAsync(projectId, project);
            if (updatedProject == null)
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            return Ok(updatedProject);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project == null)
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            return Ok(project);
        }
    }
}
