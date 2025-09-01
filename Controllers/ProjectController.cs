using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

    [HttpGet]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(new ApiResponse { Status = true, Data = projects });
        }

    [HttpGet("client/{email}")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetProjectsByClientId(string email)
        {
            var projects = await _projectService.GetProjectsByClientIdAsync(email);
            return Ok(new ApiResponse { Status = true, Data = projects });
        }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Project not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = project });
        }

    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> CreateProject(AddProjectRequestDto projectRequestDto)
        {
            var project = await _projectService.CreateProjectAsync(projectRequestDto);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.ProjectId }, new ApiResponse { Status = true, Data = project });
        }

    [HttpPut]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> UpdateProject(ProjectDTO projectDto)
        {
            var id = projectDto.ProjectId;
            var project = await _projectService.UpdateProjectAsync(id, projectDto);
            if (project == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Project not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = project });
        }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> DeactivateProject(int id)
        {
            var response = await _projectService.DeActivateProjectAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
