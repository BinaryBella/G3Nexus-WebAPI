using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
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
        
        public async Task<ApiResponse> AddProject(ProjectDTO projectDto)
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

        return new ApiResponse { Status = true, Message = "Project added successfully" };
    }

    public async Task<ApiResponse> EditProject(ProjectDTO projectDto)
    {
        var project = await _context.Projects.FindAsync(projectDto.ProjectId);
        if (project == null)
        {
            return new ApiResponse { Status = false, Message = "Project not found" };
        }

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

        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Project updated successfully" };
    }

    public async Task<ApiResponse> GetAllProjects()
    {
        var projects = await _context.Projects.ToListAsync();
        return new ApiResponse { Status = true, Data = projects };
    }

    public async Task<ApiResponse> GetProjectById(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return new ApiResponse { Status = false, Message = "Project not found" };
        }

        return new ApiResponse { Status = true, Data = project };
    }

    }
}
