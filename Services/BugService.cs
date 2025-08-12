using G3NexusBackend.Data.DTO;
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

    public async Task<IEnumerable<BugListItemDTO>> GetAllBugsAsync(int userId)
    {
        var bugs = await _context.Bugs
            .Where(b => b.IsActive)
            .Include(b => b.Client)
            .Include(b => b.Project)
            .ToListAsync();

        return bugs.Select(b => new BugListItemDTO
        {
            BugId = b.BugId,
            BugTitle = b.BugTitle,
            Severity = b.Severity,
            ClientId = b.ClientId,
            ProjectId = b.ProjectId,
            IsNew = b.IsNew,
            ClientName = b.Client.Name,
            ProjectName = b.Project.ProjectName
        });
    }
    
    public async Task<BugDTO?> GetBugByIdAsync(int bugId)
    {
        var bug = await _context.Bugs.FirstOrDefaultAsync(b => b.BugId == bugId);
        if (bug is not {IsActive: true})
        {
            return null;
        }

        bug.IsNew = false;
        _context.Bugs.Update(bug);
        await _context.SaveChangesAsync();

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
        var clientId = await _context.Clients
            .Where(c => c.Id == bugDto.ClientId && c.IsActive)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        // Validate the client exists and is active
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId && c.IsActive);
        if (!clientExists)
        {
            throw new KeyNotFoundException($"Client with ID {clientId} not found.");
        }

        // Ensure the client has an active company
        var clientCompany = await _context.Clients
            .Where(c => c.Id == clientId)
            .Include(c => c.Company)
            .Where(c => c.Company.IsActive)
            .Select(c => c.Company)
            .FirstOrDefaultAsync();
        if (clientCompany == null)
        {
            throw new KeyNotFoundException($"Company for client with ID {clientId} not found or inactive.");
        }

        // Validate the project exists and belongs to the client's company
        var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == bugDto.ProjectId && p.CompanyId == clientCompany.CompanyId && p.IsActive);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"There is no such a project belongs to your company.");
        }

        var sriLankaTime = DateTime.UtcNow.AddHours(5.5);

        var bug = new Bug
        {
            BugTitle = bugDto.BugTitle,
            Severity = bugDto.Severity,
            BugDescription = bugDto.BugDescription,
            Attachment = bugDto.Attachment,
            IsActive = true,
            ClientId = bugDto.ClientId,
            ProjectId = bugDto.ProjectId,
            CreatedAt = sriLankaTime
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
            throw new KeyNotFoundException($"Bug with ID {bugId} not found or already inactive.");
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