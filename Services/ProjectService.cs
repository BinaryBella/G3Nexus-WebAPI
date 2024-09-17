using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.Projects.Select(project => new ProjectDTO
            {
                ProjectId = project.ProjectId,
                UserId = project.UserId,
                ProjectName = project.ProjectName,
                ProjectType = project.ProjectType,
                ProjectSize = project.ProjectSize,
                CreationDate = project.CreationDate,
                ProjectDescription = project.ProjectDescription,
                EstimatedBudget = project.EstimatedBudget,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                TotalBudget = project.TotalBudget,
                Status = project.Status
            }).ToListAsync();
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return null;

            return new ProjectDTO
            {
                ProjectId = project.ProjectId,
                UserId = project.UserId,
                ProjectName = project.ProjectName,
                ProjectType = project.ProjectType,
                ProjectSize = project.ProjectSize,
                CreationDate = project.CreationDate,
                ProjectDescription = project.ProjectDescription,
                EstimatedBudget = project.EstimatedBudget,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                TotalBudget = project.TotalBudget,
                Status = project.Status
            };
        }

        public async Task AddProjectAsync(ProjectDTO projectDto)
        {
            var project = new Project
            {
                UserId = projectDto.UserId,
                ProjectName = projectDto.ProjectName,
                ProjectType = projectDto.ProjectType,
                ProjectSize = projectDto.ProjectSize,
                CreationDate = projectDto.CreationDate,
                ProjectDescription = projectDto.ProjectDescription,
                EstimatedBudget = projectDto.EstimatedBudget,
                ActualStartDate = projectDto.ActualStartDate,
                ActualEndDate = projectDto.ActualEndDate,
                TotalBudget = projectDto.TotalBudget,
                Status = projectDto.Status
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectAsync(int projectId, ProjectDTO projectDto)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return;

            project.UserId = projectDto.UserId;
            project.ProjectName = projectDto.ProjectName;
            project.ProjectType = projectDto.ProjectType;
            project.ProjectSize = projectDto.ProjectSize;
            project.CreationDate = projectDto.CreationDate;
            project.ProjectDescription = projectDto.ProjectDescription;
            project.EstimatedBudget = projectDto.EstimatedBudget;
            project.ActualStartDate = projectDto.ActualStartDate;
            project.ActualEndDate = projectDto.ActualEndDate;
            project.TotalBudget = projectDto.TotalBudget;
            project.Status = projectDto.Status;

            await _context.SaveChangesAsync();
        }
    }
}
