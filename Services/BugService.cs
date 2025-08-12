using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

public class BugService : IBugService
{
    private readonly G3NexusDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPdfGeneratorService _pdfService;

    public BugService(G3NexusDbContext context, IEmailService emailService, IPdfGeneratorService pdfService)
    {
        _context = context;
        _emailService = emailService;
        _pdfService = pdfService;
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
            ClientName = b.Client?.Name,
            ProjectName = b.Project?.ProjectName,
            IsQuoted = b.IsQuoted
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

    public async Task<ApiResponse> SendBugQuotationAsync(BugQuotationRequestDTO quotationRequest)
    {
        try
        {
            // Get bug with related entities
            var bug = await _context.Bugs
                .Include(b => b.Client)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(b => b.BugId == quotationRequest.BugId && b.IsActive);

            if (bug == null)
            {
                return new ApiResponse { Status = false, Message = "Bug not found or inactive." };
            }

            if (bug.Client == null)
            {
                return new ApiResponse { Status = false, Message = "Client information not found." };
            }

            if (bug.Project == null)
            {
                return new ApiResponse { Status = false, Message = "Project information not found." };
            }

            // Generate PDF quotation
            // TODO: Implement GenerateBugQuotation in PdfGeneratorService
            // For now, we'll use the existing RequirementQuotation method with adapted data
            var tempRequirement = new Requirement
            {
                RequirementId = bug.BugId,
                RequirementTitle = $"Bug Fix: {bug.BugTitle}",
                RequirementDescription = bug.BugDescription,
                Priority = bug.Severity,
                ClientId = bug.ClientId,
                ProjectId = bug.ProjectId,
                Client = bug.Client,
                Project = bug.Project
            };

            var pdfBytes = _pdfService.GenerateRequirementQuotation(
                tempRequirement,
                new RequirementQuotationRequestDTO 
                {
                    RequirementId = quotationRequest.BugId,
                    QuotationCost = quotationRequest.QuotationCost,
                    EstimatedDuration = quotationRequest.EstimatedDuration,
                    Description = quotationRequest.Description,
                    DeliveryDate = quotationRequest.DeliveryDate
                },
                bug.Client?.Name ?? "N/A",
                bug.Client?.ContactNo ?? "N/A",
                bug.Client?.Email ?? "N/A",
                bug.Project?.ProjectName ?? "N/A"
            );

            // Prepare Email Body
            var emailBody = await _emailService.GetEmailTemplateAsync("BugQuotation.html");
            emailBody = emailBody.Replace("{{ClientAdminName}}", bug.Client?.Name ?? "N/A")
                                 .Replace("{{BugTitle}}", bug.BugTitle ?? "N/A")
                                 .Replace("{{ProjectName}}", bug.Project?.ProjectName ?? "N/A")
                                 .Replace("{{ClientContact}}", bug.Client?.ContactNo ?? "N/A")
                                 .Replace("{{ClientEmail}}", bug.Client?.Email ?? "N/A")
                                 .Replace("{{Severity}}", bug.Severity ?? "N/A");

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                bug.Client?.Email ?? "",
                $"[Bug Quotation] {bug.BugTitle} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Bug_Quotation_{bug.BugId}.pdf",
                isHtml: true
            );

            // Update bug to mark as quoted
            bug.IsQuoted = true;
            _context.Bugs.Update(bug);
            await _context.SaveChangesAsync();

            return new ApiResponse 
            { 
                Status = true, 
                Message = "Bug quotation sent successfully to client." 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse 
            { 
                Status = false, 
                Message = $"Failed to send bug quotation: {ex.Message}" 
            };
        }
    }

    public async Task<ApiResponse> SendBulkBugQuotationAsync(BulkBugQuotationRequestDTO bulkQuotationRequest)
    {
        try
        {
            // Validate that at least one bug is selected
            if (bulkQuotationRequest.SelectedBugs == null || !bulkQuotationRequest.SelectedBugs.Any())
            {
                return new ApiResponse { Status = false, Message = "At least one bug must be selected." };
            }

            // Get all selected bugs with related entities
            var bugIds = bulkQuotationRequest.SelectedBugs.Select(sb => sb.BugId).ToList();
            var bugs = await _context.Bugs
                .Include(b => b.Client)
                .Include(b => b.Project)
                .Where(b => bugIds.Contains(b.BugId) && b.IsActive)
                .ToListAsync();

            if (bugs.Count != bulkQuotationRequest.SelectedBugs.Count)
            {
                return new ApiResponse { Status = false, Message = "One or more selected bugs not found or inactive." };
            }

            // Validate that all bugs belong to the same client and project
            var firstBug = bugs.First();
            if (bugs.Any(b => b.ClientId != firstBug.ClientId || b.ProjectId != firstBug.ProjectId))
            {
                return new ApiResponse { Status = false, Message = "All selected bugs must belong to the same client and project." };
            }

            if (firstBug.Client == null || firstBug.Project == null)
            {
                return new ApiResponse { Status = false, Message = "Client or project information not found." };
            }

            // Create quotation
            var quotation = new Quotation
            {
                ClientId = firstBug.ClientId,
                ProjectId = firstBug.ProjectId,
                CreationDate = DateTime.UtcNow.AddHours(5.5),
                Status = "Sent",
                TotalCost = bulkQuotationRequest.SelectedBugs.Sum(sb => sb.QuotationCost),
                QuotationRequirements = new List<QuotationRequirement>()
            };

            _context.Quotations?.Add(quotation);
            await _context.SaveChangesAsync();

            // Create quotation requirements (using the same table for bugs)
            var quotationRequirements = new List<QuotationRequirement>();
            foreach (var selectedBug in bulkQuotationRequest.SelectedBugs)
            {
                quotationRequirements.Add(new QuotationRequirement
                {
                    QuotationId = quotation.QuotationId,
                    RequirementId = selectedBug.BugId, // Using RequirementId field for BugId
                    RequirementCost = selectedBug.QuotationCost
                });
            }

            _context.QuotationRequirements?.AddRange(quotationRequirements);

            // Update bugs to mark as quoted and associate with quotation
            foreach (var bug in bugs)
            {
                bug.IsQuoted = true;
                bug.QuotationId = quotation.QuotationId;
            }

            _context.Bugs.UpdateRange(bugs);
            await _context.SaveChangesAsync();

            // Generate PDF quotation
            // TODO: Implement GenerateBulkBugQuotation in PdfGeneratorService
            // For now, we'll use the first bug to generate a basic PDF
            var firstSelectedBug = bulkQuotationRequest.SelectedBugs.First();
            var tempRequirement = new Requirement
            {
                RequirementId = firstBug.BugId,
                RequirementTitle = $"Bulk Bug Fixes: {bugs.Count} bugs",
                RequirementDescription = $"Multiple bug fixes. {bulkQuotationRequest.AdditionalNotes}",
                Priority = firstBug.Severity,
                ClientId = firstBug.ClientId,
                ProjectId = firstBug.ProjectId,
                Client = firstBug.Client,
                Project = firstBug.Project
            };

            var pdfBytes = _pdfService.GenerateRequirementQuotation(
                tempRequirement,
                new RequirementQuotationRequestDTO 
                {
                    RequirementId = firstSelectedBug.BugId,
                    QuotationCost = quotation.TotalCost,
                    EstimatedDuration = firstSelectedBug.EstimatedDuration,
                    Description = $"Bulk quotation for {bugs.Count} bugs. {bulkQuotationRequest.AdditionalNotes}",
                    DeliveryDate = firstSelectedBug.DeliveryDate
                },
                firstBug.Client?.Name ?? "N/A",
                firstBug.Client?.ContactNo ?? "N/A",
                firstBug.Client?.Email ?? "N/A",
                firstBug.Project?.ProjectName ?? "N/A"
            );

            // Prepare Email Body
            var emailBody = await _emailService.GetEmailTemplateAsync("BulkBugQuotation.html");
            if (string.IsNullOrEmpty(emailBody))
            {
                // Fallback to a simple email template if the bulk template doesn't exist
                emailBody = await _emailService.GetEmailTemplateAsync("BugQuotation.html");
            }

            var bugTitles = string.Join(", ", bugs.Select(b => b.BugTitle));
            emailBody = emailBody.Replace("{{ClientAdminName}}", firstBug.Client?.Name ?? "N/A")
                                 .Replace("{{BugTitle}}", bugTitles)
                                 .Replace("{{ProjectName}}", firstBug.Project?.ProjectName ?? "N/A")
                                 .Replace("{{ClientContact}}", firstBug.Client?.ContactNo ?? "N/A")
                                 .Replace("{{ClientEmail}}", firstBug.Client?.Email ?? "N/A")
                                 .Replace("{{TotalCost}}", quotation.TotalCost.ToString("C"))
                                 .Replace("{{BugCount}}", bugs.Count.ToString());

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                firstBug.Client?.Email ?? "",
                $"[Bulk Bug Quotation] {firstBug.Project?.ProjectName} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Bulk_Bug_Quotation_{quotation.QuotationId}.pdf",
                isHtml: true
            );

            return new ApiResponse 
            { 
                Status = true, 
                Message = $"Bulk bug quotation sent successfully to {firstBug.Client?.Name}. Quotation ID: {quotation.QuotationId}" 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse 
            { 
                Status = false, 
                Message = $"Failed to send bulk bug quotation: {ex.Message}" 
            };
        }
    }
}