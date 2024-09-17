using G3NexusBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public interface IProjectService
    {
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(int projectId, Project project);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(int projectId);
    }
}
