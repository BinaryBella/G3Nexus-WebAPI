using G3NexusBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
        Task<ProjectDTO> GetProjectByIdAsync(int projectId);
        Task AddProjectAsync(ProjectDTO projectDto);
        Task UpdateProjectAsync(int projectId, ProjectDTO projectDto);
    }
}