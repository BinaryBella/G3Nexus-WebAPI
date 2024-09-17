using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly G3NexusDbContext _dbContext;

        public ProjectService(G3NexusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            project.CreationDate = DateTime.Now;

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            return project;
        }

        public async Task<Project> UpdateProjectAsync(int projectId, Project project)
        {
            var existingProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (existingProject == null)
            {
                return null;
            }

            existingProject.ProjectName = project.ProjectName;
            existingProject.ProjectType = project.ProjectType;
            existingProject.ProjectSize = project.ProjectSize;
            existingProject.ProjectDescription = project.ProjectDescription;
            existingProject.EstimatedBudget = project.EstimatedBudget;
            existingProject.ActualStartDate = project.ActualStartDate;
            existingProject.ActualEndDate = project.ActualEndDate;
            existingProject.TotalBudget = project.TotalBudget;
            existingProject.Status = project.Status;

            _dbContext.Projects.Update(existingProject);
            await _dbContext.SaveChangesAsync();

            return existingProject;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }
    }
}
