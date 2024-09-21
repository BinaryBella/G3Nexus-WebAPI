using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
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

        public async Task<ApiResponse> AddBug(BugDTO bugDto)
        {
            var bug = new Bug
            {
                ProjectId = bugDto.ProjectId,
                BugTitle = bugDto.BugTitle,
                Severity = bugDto.Severity,
                BugDescription = bugDto.BugDescription,
                Attachment = bugDto.Attachment
            };

            _context.Bugs.Add(bug);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Bug added successfully" };
        }

        public async Task<ApiResponse> EditBug(BugDTO bugDto)
        {
            var bug = await _context.Bugs.FindAsync(bugDto.BugId);
            if (bug == null)
            {
                return new ApiResponse { Status = false, Message = "Bug not found" };
            }

            bug.ProjectId = bugDto.ProjectId;
            bug.BugTitle = bugDto.BugTitle;
            bug.Severity = bugDto.Severity;
            bug.BugDescription = bugDto.BugDescription;
            bug.Attachment = bugDto.Attachment;

            _context.Bugs.Update(bug);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Bug updated successfully" };
        }

        public async Task<ApiResponse> GetAllBugs()
        {
            var bugs = await _context.Bugs.ToListAsync();
            return new ApiResponse { Status = true, Data = bugs };
        }

        public async Task<ApiResponse> GetBugById(int bugId)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null)
            {
                return new ApiResponse { Status = false, Message = "Bug not found" };
            }

            return new ApiResponse { Status = true, Data = bug };
        }
    }
}
