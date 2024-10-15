using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
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

        public async Task<IEnumerable<RequirementDTO>> GetAllRequirementsAsync()
        {
            return await _context.Requirements
                .Where(r => r.IsActive) 
                .Select(r => new RequirementDTO
                {
                    RequirementId = r.RequirementId,
                    RequirementTitle = r.RequirementTitle,
                    Priority = r.Priority,
                    RequirementDescription = r.RequirementDescription,
                    Attachment = r.Attachment,
                    IsActive = r.IsActive,
                    ClientId = r.ClientId,
                    ProjectId = r.ProjectId
                })
                .ToListAsync();
        }

        public async Task<RequirementDTO> GetRequirementByIdAsync(int requirementId)
        {
            var requirement = await _context.Requirements.FindAsync(requirementId);
            if (requirement == null || !requirement.IsActive)
            {
                return null;
            }

            return new RequirementDTO
            {
                RequirementId = requirement.RequirementId,
                RequirementTitle = requirement.RequirementTitle,
                Priority = requirement.Priority,
                RequirementDescription = requirement.RequirementDescription,
                Attachment = requirement.Attachment,
                IsActive = requirement.IsActive,
                ClientId = requirement.ClientId,
                ProjectId = requirement.ProjectId
            };
        }

        public async Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto)
        {
            var requirement = new Requirement
            {
                RequirementTitle = requirementDto.RequirementTitle,
                Priority = requirementDto.Priority,
                RequirementDescription = requirementDto.RequirementDescription,
                Attachment = requirementDto.Attachment,
                IsActive = true, // New requirement is active by default
                ClientId = requirementDto.ClientId,
                ProjectId = requirementDto.ProjectId
            };

            _context.Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            requirementDto.RequirementId = requirement.RequirementId;
            return requirementDto;
        }

        public async Task<RequirementDTO> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto)
        {
            var requirement = await _context.Requirements.FindAsync(requirementId);
            if (requirement == null || !requirement.IsActive)
            {
                return null;
            }

            requirement.RequirementTitle = requirementDto.RequirementTitle;
            requirement.Priority = requirementDto.Priority;
            requirement.RequirementDescription = requirementDto.RequirementDescription;
            requirement.Attachment = requirementDto.Attachment;
            requirement.ClientId = requirementDto.ClientId;
            requirement.ProjectId = requirementDto.ProjectId;

            _context.Requirements.Update(requirement);
            await _context.SaveChangesAsync();

            return requirementDto;
        }

        public async Task<ApiResponse> DeActivateRequirementAsync(int requirementId)
        {
            var requirement = await _context.Requirements.FindAsync(requirementId);
            if (requirement == null || !requirement.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Requirement not found or already inactive." };
            }

            requirement.IsActive = false;
            _context.Requirements.Update(requirement);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Requirement successfully deactivated." };
        }
    }
}
