using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using G3NexusBackend.Models;
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
            ClientName = r.Client?.Name,
            ProjectName = r.Project?.ProjectName,
            IsQuoted = r.IsQuoted
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

    public async Task<ApiResponse> SendBulkQuotationAsync(BulkQuotationRequestDTO bulkQuotationRequest)
    {
        try
        {
            // Validate that at least one requirement is selected
            if (bulkQuotationRequest.SelectedRequirements == null || !bulkQuotationRequest.SelectedRequirements.Any())
            {
                return new ApiResponse { Status = false, Message = "At least one requirement must be selected." };
            }

            // Get all selected requirements with related entities
            var requirementIds = bulkQuotationRequest.SelectedRequirements.Select(sr => sr.RequirementId).ToList();
            var requirements = await _context.Requirements
                .Include(r => r.Client)
                .Include(r => r.Project)
                .Where(r => requirementIds.Contains(r.RequirementId) && r.IsActive)
                .ToListAsync();

            if (requirements.Count != bulkQuotationRequest.SelectedRequirements.Count)
            {
                return new ApiResponse { Status = false, Message = "One or more selected requirements not found or inactive." };
            }

            // Validate that all requirements belong to the same client and project
            var firstRequirement = requirements.First();
            if (requirements.Any(r => r.ClientId != firstRequirement.ClientId || r.ProjectId != firstRequirement.ProjectId))
            {
                return new ApiResponse { Status = false, Message = "All selected requirements must belong to the same client and project." };
            }

            if (firstRequirement.Client == null || firstRequirement.Project == null)
            {
                return new ApiResponse { Status = false, Message = "Client or project information not found." };
            }

            // Create quotation
            var quotation = new Quotation
            {
                ClientId = firstRequirement.ClientId,
                ProjectId = firstRequirement.ProjectId,
                CreationDate = DateTime.UtcNow.AddHours(5.5),
                Status = "Sent",
                TotalCost = bulkQuotationRequest.SelectedRequirements.Sum(sr => sr.QuotationCost),
                QuotationRequirements = new List<QuotationRequirement>()
            };

            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();

            // Create quotation requirements
            var quotationRequirements = new List<QuotationRequirement>();
            foreach (var selectedReq in bulkQuotationRequest.SelectedRequirements)
            {
                quotationRequirements.Add(new QuotationRequirement
                {
                    QuotationId = quotation.QuotationId,
                    RequirementId = selectedReq.RequirementId,
                    RequirementCost = selectedReq.QuotationCost
                });
            }

            _context.QuotationRequirements?.AddRange(quotationRequirements);

            // Update requirements to mark as quoted and associate with quotation
            foreach (var requirement in requirements)
            {
                requirement.IsQuoted = true;
                requirement.QuotationId = quotation.QuotationId;
            }

            _context.Requirements.UpdateRange(requirements);
            await _context.SaveChangesAsync();

            // Generate PDF quotation
            // TODO: Implement GenerateBulkRequirementQuotation in PdfGeneratorService
            // For now, we'll use the first requirement to generate a basic PDF
            var firstSelectedReq = bulkQuotationRequest.SelectedRequirements.First();
            var pdfBytes = _pdfService.GenerateRequirementQuotation(
                firstRequirement,
                new RequirementQuotationRequestDTO 
                {
                    RequirementId = firstSelectedReq.RequirementId,
                    QuotationCost = quotation.TotalCost,
                    EstimatedDuration = firstSelectedReq.EstimatedDuration,
                    Description = $"Bulk quotation for {requirements.Count} requirements. {bulkQuotationRequest.AdditionalNotes}",
                    DeliveryDate = firstSelectedReq.DeliveryDate
                },
                firstRequirement.Client?.ContactNo ?? "N/A",
                firstRequirement.Client?.Email ?? "N/A",
                firstRequirement.Project?.ProjectName ?? "N/A"
            );

            // Prepare Email Body
            var emailBody = await _emailService.GetEmailTemplateAsync("BulkRequirementQuotation.html");
            if (string.IsNullOrEmpty(emailBody))
            {
                // Fallback to a simple email template if the bulk template doesn't exist
                emailBody = await _emailService.GetEmailTemplateAsync("RequirementQuotation.html");
            }

            var requirementTitles = string.Join(", ", requirements.Select(r => r.RequirementTitle));
            emailBody = emailBody.Replace("{{ClientAdminName}}", firstRequirement.Client?.Name ?? "N/A")
                                 .Replace("{{RequirementTitle}}", requirementTitles)
                                 .Replace("{{ProjectName}}", firstRequirement.Project?.ProjectName ?? "N/A")
                                 .Replace("{{ClientContact}}", firstRequirement.Client?.ContactNo ?? "N/A")
                                 .Replace("{{ClientEmail}}", firstRequirement.Client?.Email ?? "N/A")
                                 .Replace("{{TotalCost}}", quotation.TotalCost.ToString("C"))
                                 .Replace("{{RequirementCount}}", requirements.Count.ToString());

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                firstRequirement.Client?.Email ?? "",
                $"[Bulk Quotation] {firstRequirement.Project?.ProjectName} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Bulk_Quotation_{quotation.QuotationId}.pdf",
                isHtml: true
            );

            return new ApiResponse 
            { 
                Status = true, 
                Message = $"Bulk quotation sent successfully to {firstRequirement.Client?.Name}. Quotation ID: {quotation.QuotationId}" 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse 
            { 
                Status = false, 
                Message = $"Failed to send bulk quotation: {ex.Message}" 
            };
        }
    }
}