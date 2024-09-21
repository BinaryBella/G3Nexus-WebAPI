using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace G3NexusBackend.Services
{
    public class RequirementService : IRequirementService
    {
        private readonly G3NexusDbContext _context;

        public RequirementService(G3NexusDbContext context)
        {
            _context = context;
        }

         public async Task<ApiResponse> AddRequirement(RequirementDTO requirementDto)
    {
        var requirement = new Requirement
        {
            ProjectId = requirementDto.ProjectId,
            RequirementTitle = requirementDto.RequirementTitle,
            Priority = requirementDto.Priority,
            RequirementDescription = requirementDto.RequirementDescription,
            Attachment = requirementDto.Attachment
        };

        _context.Requirements.Add(requirement);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Requirement added successfully" };
    }

    public async Task<ApiResponse> EditRequirement(RequirementDTO requirementDto)
    {
        var requirement = await _context.Requirements.FindAsync(requirementDto.RequirementId);
        if (requirement == null)
        {
            return new ApiResponse { Status = false, Message = "Requirement not found" };
        }

        requirement.ProjectId = requirementDto.ProjectId;
        requirement.RequirementTitle = requirementDto.RequirementTitle;
        requirement.Priority = requirementDto.Priority;
        requirement.RequirementDescription = requirementDto.RequirementDescription;
        requirement.Attachment = requirementDto.Attachment;

        _context.Requirements.Update(requirement);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Requirement updated successfully" };
    }

    public async Task<ApiResponse> GetAllRequirements()
    {
        var requirements = await _context.Requirements.ToListAsync();
        return new ApiResponse { Status = true, Data = requirements };
    }

    public async Task<ApiResponse> GetRequirementById(int requirementId)
    {
        var requirement = await _context.Requirements.FindAsync(requirementId);
        if (requirement == null)
        {
            return new ApiResponse { Status = false, Message = "Requirement not found" };
        }

        return new ApiResponse { Status = true, Data = requirement };
    }
    }
}
