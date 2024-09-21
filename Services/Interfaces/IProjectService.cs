using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IProjectService
    {
        Task<ApiResponse> AddProject(ProjectDTO projectDto);
        Task<ApiResponse> EditProject(ProjectDTO projectDto);
        Task<ApiResponse> GetAllProjects();
        Task<ApiResponse> GetProjectById(int projectId);
    }
}