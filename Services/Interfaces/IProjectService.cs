using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
        Task<ProjectDTO> GetProjectByIdAsync(int projectId);
        Task<ProjectDTO> CreateProjectAsync(ProjectDTO projectDto);
        Task<ProjectDTO> UpdateProjectAsync(int projectId, ProjectDTO projectDto);
        Task<ApiResponse> DeActivateProjectAsync(int projectId); 
    }
}