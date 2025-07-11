using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
    Task<ProjectDTO?> GetProjectByIdAsync(int projectId);
    Task<ProjectDTO> CreateProjectAsync(ProjectDTO projectDto);
    Task<ProjectDTO?> UpdateProjectAsync(int projectId, ProjectDTO projectDto);
    Task<ApiResponse> DeActivateProjectAsync(int projectId);
    Task<IEnumerable<ProjectDTO>> GetProjectsByClientIdAsync(string email);
}