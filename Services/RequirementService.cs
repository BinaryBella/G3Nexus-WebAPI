using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

public class RequirementService : IRequirementService
{
    private readonly G3NexusDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPdfGeneratorService _pdfService;

    public RequirementService(G3NexusDbContext context, IEmailService emailService, IPdfGeneratorService pdfService)
    {
        _context = context;
        _emailService = emailService;
        _pdfService = pdfService;
    }

    public async Task<IEnumerable<RequirementListItemDTO>> GetAllRequirementsAsync()
    {
        var requirements = await _context.Requirements
            .Where(r => r.IsActive)
            .Include(r => r.Client)
            .Include(r => r.Project)
            .ToListAsync();

        return requirements.Select(r => new RequirementListItemDTO
        {
            RequirementId = r.RequirementId,
            RequirementTitle = r.RequirementTitle,
            Priority = r.Priority,
            ClientId = r.ClientId,
            ProjectId = r.ProjectId,
            IsNew = r.IsNew,
            ClientName = r.Client.Name,
            ProjectName = r.Project.ProjectName
        });
    }

    public async Task<RequirementDTO?> GetRequirementByIdAsync(int requirementId)
    {    
        var requirement = await _context.Requirements.FirstOrDefaultAsync(r => r.RequirementId == requirementId);
        if (requirement == null || !requirement.IsActive)
        {
            return null;
        }

        requirement.IsNew = false;
        _context.Requirements.Update(requirement);
        await _context.SaveChangesAsync();

        return new RequirementDTO
        {
            RequirementId = requirement.RequirementId,
            RequirementTitle = requirement.RequirementTitle,
            Priority = requirement.Priority,
            RequirementDescription = requirement.RequirementDescription,
            Attachment = requirement.Attachment,
            IsActive = requirement.IsActive,
            ClientId = requirement.ClientId,
            ProjectId = requirement.ProjectId,
            IsNew = false
        };
    }

    public async Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto)
    {
        var clientId = await _context.Clients
            .Where(c => c.Id == requirementDto.ClientId && c.IsActive)
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
        var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == requirementDto.ProjectId && p.CompanyId == clientCompany.CompanyId && p.IsActive);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"There is no such a project belongs to your company.");
        }

        var sriLankaTime = DateTime.UtcNow.AddHours(5.5);

        var requirement = new Requirement
        {
            RequirementTitle = requirementDto.RequirementTitle,
            Priority = requirementDto.Priority,
            RequirementDescription = requirementDto.RequirementDescription,
            Attachment = requirementDto.Attachment,
            IsActive = true,
            ClientId = clientId,
            ProjectId = requirementDto.ProjectId,
            CreatedAt = sriLankaTime,
            IsNew = true
        };

        _context.Requirements.Add(requirement);
        await _context.SaveChangesAsync();

        requirementDto.RequirementId = requirement.RequirementId;
        return requirementDto;
    }

    public async Task<RequirementDTO?> UpdateRequirementAsync(int requirementId, RequirementDTO requirementDto)
    {
        var requirement = await _context.Requirements.FindAsync(requirementId);
        if (requirement is not {IsActive: true})
        {
            throw new KeyNotFoundException($"Requirement with ID {requirementId} not found or already inactive.");
        }

        var clientExists = await _context.Clients.AnyAsync(c => c.Id == requirementDto.ClientId);
        if (!clientExists)
        {
            throw new KeyNotFoundException($"Client with ID {requirementDto.ClientId} not found.");
        }

        var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == requirementDto.ProjectId);
        if (!projectExists)
        {
            throw new KeyNotFoundException($"Project with ID {requirementDto.ProjectId} not found.");
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

    public async Task<ApiResponse> SendRequirementQuotationAsync(RequirementQuotationRequestDTO quotationRequest)
    {
        try
        {
            // Get requirement with related entities
            var requirement = await _context.Requirements
                .Include(r => r.Client)
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.RequirementId == quotationRequest.RequirementId && r.IsActive);

            if (requirement == null)
            {
                return new ApiResponse { Status = false, Message = "Requirement not found or inactive." };
            }

            if (requirement.Client == null)
            {
                return new ApiResponse { Status = false, Message = "Client information not found." };
            }

            if (requirement.Project == null)
            {
                return new ApiResponse { Status = false, Message = "Project information not found." };
            }

            // Generate PDF quotation
            var pdfBytes = _pdfService.GenerateRequirementQuotation(
                requirement,
                quotationRequest,
                requirement.Client.Name ?? "N/A",
                requirement.Client.ContactNo ?? "N/A",
                requirement.Client.Email ?? "N/A",
                requirement.Project.ProjectName ?? "N/A"
            );

            // Prepare Email Body
            var emailBody = await _emailService.GetEmailTemplateAsync("RequirementQuotation.html");
            emailBody = emailBody.Replace("{{ClientAdminName}}", requirement.Client.Name ?? "N/A")
                                 .Replace("{{RequirementTitle}}", requirement.RequirementTitle ?? "N/A")
                                 .Replace("{{ProjectName}}", requirement.Project.ProjectName ?? "N/A")
                                 .Replace("{{ClientContact}}", requirement.Client.ContactNo ?? "N/A")
                                 .Replace("{{ClientEmail}}", requirement.Client.Email ?? "N/A")
                                 .Replace("{{Priority}}", requirement.Priority ?? "N/A");

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                requirement.Client.Email ?? "",
                $"[Requirement Quotation] {requirement.RequirementTitle} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Requirement_Quotation_{requirement.RequirementId}.pdf",
                isHtml: true
            );

            // Update requirement to mark as quoted
            requirement.IsQuoted = true;
            _context.Requirements.Update(requirement);
            await _context.SaveChangesAsync();

            return new ApiResponse 
            { 
                Status = true, 
                Message = "Requirement quotation sent successfully to client." 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse 
            { 
                Status = false, 
                Message = $"Failed to send requirement quotation: {ex.Message}" 
            };
        }
    }
}