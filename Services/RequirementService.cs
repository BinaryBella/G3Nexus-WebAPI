using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.Requirements.Select(req => new RequirementDTO
            {
                RequirementId = req.RequirementId,
                ProjectId = req.ProjectId,
                RequirementTitle = req.RequirementTitle,
                Priority = req.Priority,
                RequirementDescription = req.RequirementDescription,
                Attachment = req.Attachment
            }).ToListAsync();
        }

        public async Task<RequirementDTO> GetRequirementByIdAsync(int requirementId)
        {
            var req = await _context.Requirements.FindAsync(requirementId);
            if (req == null) return null;

            return new RequirementDTO
            {
                RequirementId = req.RequirementId,
                ProjectId = req.ProjectId,
                RequirementTitle = req.RequirementTitle,
                Priority = req.Priority,
                RequirementDescription = req.RequirementDescription,
                Attachment = req.Attachment
            };
        }

        public async Task AddRequirementAsync(RequirementDTO requirementDto)
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
        }

        public async Task UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto)
        {
            var req = await _context.Requirements.FindAsync(requirementId);
            if (req == null) return;

            req.ProjectId = requirementDto.ProjectId;
            req.RequirementTitle = requirementDto.RequirementTitle;
            req.Priority = requirementDto.Priority;
            req.RequirementDescription = requirementDto.RequirementDescription;
            req.Attachment = requirementDto.Attachment;

            await _context.SaveChangesAsync();
        }
    }
}
