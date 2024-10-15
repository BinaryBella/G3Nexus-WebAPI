using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services
{
    public class BugService : IBugService
    {
        private readonly G3NexusDbContext _context;

        public BugService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BugDTO>> GetAllBugsAsync()
        {
            return await _context.Bugs
                .Where(b => b.IsActive) // Only return active bugs
                .Select(b => new BugDTO
                {
                    BugId = b.BugId,
                    BugTitle = b.BugTitle,
                    Severity = b.Severity,
                    BugDescription = b.BugDescription,
                    Attachment = b.Attachment,
                    IsActive = b.IsActive,
                    ClientId = b.ClientId,
                    ProjectId = b.ProjectId
                })
                .ToListAsync();
        }

        public async Task<BugDTO> GetBugByIdAsync(int bugId)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null || !bug.IsActive)
            {
                return null;
            }

            return new BugDTO
            {
                BugId = bug.BugId,
                BugTitle = bug.BugTitle,
                Severity = bug.Severity,
                BugDescription = bug.BugDescription,
                Attachment = bug.Attachment,
                IsActive = bug.IsActive,
                ClientId = bug.ClientId,
                ProjectId = bug.ProjectId
            };
        }

        public async Task<BugDTO> CreateBugAsync(BugDTO bugDto)
        {
            var bug = new Bug
            {
                BugTitle = bugDto.BugTitle,
                Severity = bugDto.Severity,
                BugDescription = bugDto.BugDescription,
                Attachment = bugDto.Attachment,
                IsActive = true, // Newly created bug is active by default
                ClientId = bugDto.ClientId,
                ProjectId = bugDto.ProjectId
            };

            _context.Bugs.Add(bug);
            await _context.SaveChangesAsync();

            bugDto.BugId = bug.BugId;
            return bugDto;
        }

        public async Task<BugDTO> UpdateBugAsync(int bugId, BugDTO bugDto)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null || !bug.IsActive)
            {
                return null;
            }

            bug.BugTitle = bugDto.BugTitle;
            bug.Severity = bugDto.Severity;
            bug.BugDescription = bugDto.BugDescription;
            bug.Attachment = bugDto.Attachment;
            bug.ClientId = bugDto.ClientId;
            bug.ProjectId = bugDto.ProjectId;

            _context.Bugs.Update(bug);
            await _context.SaveChangesAsync();

            return bugDto;
        }

        public async Task<ApiResponse> DeActivateBugAsync(int bugId)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null || !bug.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Bug not found or already inactive." };
            }

            bug.IsActive = false;
            _context.Bugs.Update(bug);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Bug successfully deactivated." };
        }
    }
}
