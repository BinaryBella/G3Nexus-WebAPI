using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.Bugs.Select(bug => new BugDTO
            {
                BugId = bug.BugId,
                ProjectId = bug.ProjectId,
                BugTitle = bug.BugTitle,
                Severity = bug.Severity,
                BugDescription = bug.BugDescription,
                Attachment = bug.Attachment
            }).ToListAsync();
        }

        public async Task<BugDTO> GetBugByIdAsync(int bugId)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null) return null;

            return new BugDTO
            {
                BugId = bug.BugId,
                ProjectId = bug.ProjectId,
                BugTitle = bug.BugTitle,
                Severity = bug.Severity,
                BugDescription = bug.BugDescription,
                Attachment = bug.Attachment
            };
        }

        public async Task AddBugAsync(BugDTO bugDto)
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
        }

        public async Task UpdateBugAsync(int bugId, BugDTO bugDto)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null) return;

            bug.ProjectId = bugDto.ProjectId;
            bug.BugTitle = bugDto.BugTitle;
            bug.Severity = bugDto.Severity;
            bug.BugDescription = bugDto.BugDescription;
            bug.Attachment = bugDto.Attachment;

            await _context.SaveChangesAsync();
        }
    }
}
