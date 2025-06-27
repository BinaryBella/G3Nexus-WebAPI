using G3NexusBackend.DTOs;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

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
            .Where(b => b.IsActive)
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

    public async Task<BugDTO?> GetBugByIdAsync(int bugId)
    {
        var bug = await _context.Bugs.FindAsync(bugId);
        if (bug is not {IsActive: true})
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
        // Check if client exists
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == bugDto.ClientId);
        if (!clientExists)
        {
            throw new KeyNotFoundException($"Client with ID {bugDto.ClientId} not found.");
        }

        // Check if project exists
        var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == bugDto.ProjectId);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"Project with ID {bugDto.ProjectId} not found.");
        }

        var bug = new Bug
        {
            BugTitle = bugDto.BugTitle,
            Severity = bugDto.Severity,
            BugDescription = bugDto.BugDescription,
            Attachment = bugDto.Attachment,
            IsActive = true,
            ClientId = bugDto.ClientId,
            ProjectId = bugDto.ProjectId
        };

        _context.Bugs.Add(bug);
        await _context.SaveChangesAsync();

        bugDto.BugId = bug.BugId;
        return bugDto;
    }

    public async Task<BugDTO?> UpdateBugAsync(int bugId, BugDTO bugDto)
    {
        var bug = await _context.Bugs.FindAsync(bugId);
        if (bug is not {IsActive: true})
        {
            return null;
        }

        var clientExists = await _context.Clients.AnyAsync(c => c.Id == bugDto.ClientId);
        if (!clientExists)
        {
            throw new KeyNotFoundException($"Client with ID {bugDto.ClientId} not found.");
        }

        var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == bugDto.ProjectId);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"Project with ID {bugDto.ProjectId} not found.");
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
        if (bug is not {IsActive: true})
        {
            return new ApiResponse { Status = false, Message = "Bug not found or already inactive." };
        }

        bug.IsActive = false;
        _context.Bugs.Update(bug);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Bug successfully deactivated." };
    }
}