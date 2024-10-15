using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly G3NexusDbContext _context;

        public ProjectService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Where(p => p.IsActive)
                .Select(p => new ProjectDTO
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    ProjectType = p.ProjectType,
                    ProjectSize = p.ProjectSize,
                    CreationDate = p.CreationDate,
                    ProjectDescription = p.ProjectDescription,
                    EstimatedBudget = p.EstimatedBudget,
                    ActualStartDate = p.ActualStartDate,
                    ActualEndDate = p.ActualEndDate,
                    TotalBudget = p.TotalBudget,
                    PaymentType = p.PaymentType,
                    PaymentStatus = p.PaymentStatus,
                    Status = p.Status,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || !project.IsActive)
            {
                return null;
            }

            return new ProjectDTO
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectType = project.ProjectType,
                ProjectSize = project.ProjectSize,
                CreationDate = project.CreationDate,
                ProjectDescription = project.ProjectDescription,
                EstimatedBudget = project.EstimatedBudget,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                TotalBudget = project.TotalBudget,
                PaymentType = project.PaymentType,
                PaymentStatus = project.PaymentStatus,
                Status = project.Status,
                IsActive = project.IsActive
            };
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectDTO projectDto)
        {
            var project = new Project
            {
                ProjectName = projectDto.ProjectName,
                ProjectType = projectDto.ProjectType,
                ProjectSize = projectDto.ProjectSize,
                CreationDate = projectDto.CreationDate,
                ProjectDescription = projectDto.ProjectDescription,
                EstimatedBudget = projectDto.EstimatedBudget,
                ActualStartDate = projectDto.ActualStartDate,
                ActualEndDate = projectDto.ActualEndDate,
                TotalBudget = projectDto.TotalBudget,
                PaymentType = projectDto.PaymentType,
                PaymentStatus = projectDto.PaymentStatus,
                Status = projectDto.Status,
                IsActive = true // New projects are active by default
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            projectDto.ProjectId = project.ProjectId;
            return projectDto;
        }

        public async Task<ProjectDTO> UpdateProjectAsync(int projectId, ProjectDTO projectDto)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || !project.IsActive)
            {
                return null;
            }

            project.ProjectName = projectDto.ProjectName;
            project.ProjectType = projectDto.ProjectType;
            project.ProjectSize = projectDto.ProjectSize;
            project.CreationDate = projectDto.CreationDate;
            project.ProjectDescription = projectDto.ProjectDescription;
            project.EstimatedBudget = projectDto.EstimatedBudget;
            project.ActualStartDate = projectDto.ActualStartDate;
            project.ActualEndDate = projectDto.ActualEndDate;
            project.TotalBudget = projectDto.TotalBudget;
            project.PaymentType = projectDto.PaymentType;
            project.PaymentStatus = projectDto.PaymentStatus;
            project.Status = projectDto.Status;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return projectDto;
        }

        public async Task<ApiResponse> DeActivateProjectAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || !project.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Project not found or already inactive." };
            }

            project.IsActive = false;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Project successfully deactivated." };
        }
    }
}
