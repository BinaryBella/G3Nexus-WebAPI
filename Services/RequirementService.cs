using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Data;

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
            Status = r.Status,
            ClientName = r.Client?.Name,
            ProjectName = r.Project?.ProjectName,
            IsQuoted = r.IsQuoted
        });
    }

    public async Task<IEnumerable<RequirementListItemDTO>> GetRequirementsByProjectAsync(RequirementsByProjectRequestDTO request)
    {
        var requirements = await _context.Requirements
            .Where(r => r.IsActive && r.ClientId == request.ClientId && r.ProjectId == request.ProjectId)
            .Where(r => request.IncludeQuoted || !r.IsQuoted) // Filter by quotation status
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
            Status = r.Status,
            ClientName = r.Client?.Name,
            ProjectName = r.Project?.ProjectName,
            IsQuoted = r.IsQuoted
        });
    }

    public async Task<ApiResponse> ValidateBulkQuotationSelectionAsync(BulkQuotationValidationDTO validation)
    {
        try
        {
            if (!validation.RequirementIds.Any())
            {
                return new ApiResponse { Status = false, Message = "No requirements selected for validation." };
            }

            // Check if all requirements exist and are active
            var requirements = await _context.Requirements
                .Where(r => validation.RequirementIds.Contains(r.RequirementId) && r.IsActive)
                .ToListAsync();

            var foundIds = requirements.Select(r => r.RequirementId).ToList();
            var missingIds = validation.RequirementIds.Except(foundIds).ToList();

            if (missingIds.Any())
            {
                return new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Requirements not found or inactive: {string.Join(", ", missingIds)}" 
                };
            }

            // Check if any are already quoted
            var quotedRequirements = requirements.Where(r => r.IsQuoted).ToList();
            if (quotedRequirements.Any())
            {
                var quotedIds = quotedRequirements.Select(r => r.RequirementId).ToList();
                return new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Requirements already quoted: {string.Join(", ", quotedIds)}" 
                };
            }

            // Check if all belong to the same client and project
            var distinctClients = requirements.Select(r => r.ClientId).Distinct().ToList();
            var distinctProjects = requirements.Select(r => r.ProjectId).Distinct().ToList();

            if (distinctClients.Count > 1)
            {
                return new ApiResponse { Status = false, Message = "All requirements must belong to the same client." };
            }

            if (distinctProjects.Count > 1)
            {
                return new ApiResponse { Status = false, Message = "All requirements must belong to the same project." };
            }

            // Validate against the provided client and project IDs
            if (distinctClients.First() != validation.ClientId)
            {
                return new ApiResponse { Status = false, Message = "Requirements do not belong to the specified client." };
            }

            if (distinctProjects.First() != validation.ProjectId)
            {
                return new ApiResponse { Status = false, Message = "Requirements do not belong to the specified project." };
            }

            return new ApiResponse 
            { 
                Status = true, 
                Message = $"All {requirements.Count} requirements are valid for bulk quotation." 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse 
            { 
                Status = false, 
                Message = $"Validation failed: {ex.Message}" 
            };
        }
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
            Status = requirement.Status,
            ClientId = requirement.ClientId,
            ProjectId = requirement.ProjectId,
            IsNew = false
        };
    }

    public async Task<RequirementDTO> CreateRequirementAsync(RequirementDTO requirementDto)
    {
        var clientId = await _context.Clients
            .Where(c => c.ClientId == requirementDto.ClientId && c.IsActive)
            .Select(c => c.ClientId)
            .FirstOrDefaultAsync();

        // Validate the client exists and is active
        var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == clientId && c.IsActive);
        if (!clientExists)
        {
            throw new KeyNotFoundException($"Client with ID {clientId} not found.");
        }

        // Ensure the client has an active company
        var clientCompany = await _context.Clients
            .Where(c => c.ClientId == clientId)
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

        var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == requirementDto.ClientId);
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
        requirement.Status = requirementDto.Status;
        requirement.ClientId = requirementDto.ClientId;
        requirement.ProjectId = requirementDto.ProjectId;

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
            IsNew = requirement.IsNew,
            Status = requirement.Status,
            ClientId = requirement.ClientId,
            ProjectId = requirement.ProjectId
        };
    }

    public async Task<ApiResponse> UpdateRequirementStatusAsync(int requirementId, G3NexusBackend.Models.TaskStatus status)
    {
        var requirement = await _context.Requirements.FindAsync(requirementId);
        if (requirement == null || !requirement.IsActive)
        {
            return new ApiResponse { Status = false, Message = "Requirement not found or already inactive." };
        }

        requirement.Status = status;
        _context.Requirements.Update(requirement);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Requirement status updated successfully." };
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

            // Validate EmployeeId exists
            if (_context.Employees == null)
            {
                return new ApiResponse { Status = false, Message = "Database context error." };
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == quotationRequest.EmployeeId && e.IsActive);

            if (employee == null)
            {
                return new ApiResponse { Status = false, Message = "Employee not found or inactive." };
            }

            if (requirement.Client == null)
            {
                return new ApiResponse { Status = false, Message = "Client information not found." };
            }

            if (requirement.Project == null)
            {
                return new ApiResponse { Status = false, Message = "Project information not found." };
            }

            // Create quotation first to get QuotationId
            var quotation = new Quotation
            {
                ClientId = requirement.ClientId,
                ProjectId = requirement.ProjectId,
                EmployeeId = quotationRequest.EmployeeId,
                CreatedDate = DateTime.UtcNow.AddHours(5.5),
                Type = Constants.RequirementQuotationType,
                TotalCost = quotationRequest.QuotationCost
            };

            _context.Quotations?.Add(quotation);
            await _context.SaveChangesAsync(); // Save to get QuotationId

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
                                 .Replace("{{Priority}}", requirement.Priority ?? "N/A")
                                 .Replace("{{QuotationId}}", quotation.QuotationId.ToString());

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                requirement.Client.Email ?? "",
                $"[Requirement Quotation] {requirement.RequirementTitle} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Requirement_Quotation_{quotation.QuotationId}.pdf",
                isHtml: true
            );

            // Update requirement to mark as quoted and link to quotation
            requirement.IsQuoted = true;
            requirement.QuotationId = quotation.QuotationId;
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
        // Use Entity Framework's execution strategy for proper transaction handling
        var strategy = _context.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate input
                if (bulkQuotationRequest.SelectedRequirements == null || !bulkQuotationRequest.SelectedRequirements.Any())
                {
                    return new ApiResponse { Status = false, Message = "At least one requirement must be selected." };
                }

                // 2. Validate business rules
                var validationResult = ValidateBulkQuotationRequest(bulkQuotationRequest);
                if (!validationResult.IsValid)
                {
                    return new ApiResponse { Status = false, Message = validationResult.ErrorMessage };
                }

                // 3. Get all selected requirements with related entities in a single query
                var requirementIds = bulkQuotationRequest.SelectedRequirements.Select(sr => sr.RequirementId).ToList();
                
                if (_context.Requirements == null)
                {
                    return new ApiResponse { Status = false, Message = "Database context error." };
                }

                var requirements = await _context.Requirements
                    .Include(r => r.Client)
                    .Include(r => r.Project)
                    .Where(r => requirementIds.Contains(r.RequirementId) && r.IsActive && !r.IsQuoted)
                    .ToListAsync();

                // 4. Validate all requirements were found and not already quoted
                if (requirements.Count != bulkQuotationRequest.SelectedRequirements.Count)
                {
                    var foundIds = requirements.Select(r => r.RequirementId).ToList();
                    var missingIds = requirementIds.Except(foundIds).ToList();
                    return new ApiResponse 
                    { 
                        Status = false, 
                        Message = $"Requirements not found or already quoted: {string.Join(", ", missingIds)}" 
                    };
                }

                // 5. Validate business consistency
                var firstRequirement = requirements.First();
                if (requirements.Any(r => r.ClientId != firstRequirement.ClientId || r.ProjectId != firstRequirement.ProjectId))
                {
                    return new ApiResponse { Status = false, Message = "All selected requirements must belong to the same client and project." };
                }

                if (firstRequirement.Client == null || firstRequirement.Project == null)
                {
                    return new ApiResponse { Status = false, Message = "Client or project information not found." };
                }

                // 5.5. Validate EmployeeId exists
                if (_context.Employees == null)
                {
                    return new ApiResponse { Status = false, Message = "Database context error." };
                }

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == bulkQuotationRequest.EmployeeId && e.IsActive);

                if (employee == null)
                {
                    return new ApiResponse { Status = false, Message = "Employee not found or inactive." };
                }

                // 6. Calculate total cost from selected requirements
                var totalCost = bulkQuotationRequest.SelectedRequirements.Sum(sr => sr.QuotationCost);
                if (totalCost <= 0)
                {
                    return new ApiResponse { Status = false, Message = "Total quotation cost must be greater than zero." };
                }

                // 7. Create quotation master record
                var quotation = new Quotation
                {
                    ClientId = firstRequirement.ClientId,
                    ProjectId = firstRequirement.ProjectId,
                    EmployeeId = bulkQuotationRequest.EmployeeId,
                    CreatedDate = DateTime.UtcNow.AddHours(5.5),
                    Type = Constants.RequirementQuotationType,
                    TotalCost = totalCost
                };

                _context.Quotations?.Add(quotation);
                await _context.SaveChangesAsync(); // Save to get QuotationId

                // 9. Update requirements to mark as quoted and associate with quotation
                foreach (var requirement in requirements)
                {
                    requirement.IsQuoted = true;
                    requirement.QuotationId = quotation.QuotationId;
                }

                _context.Requirements?.UpdateRange(requirements);
                await _context.SaveChangesAsync();

                // 10. Generate comprehensive PDF quotation
                var pdfBytes = GenerateBulkQuotationPdf(requirements, bulkQuotationRequest, firstRequirement, quotation);

                // 11. Send email with quotation
                var emailResult = await SendBulkQuotationEmail(requirements, bulkQuotationRequest, firstRequirement, quotation, pdfBytes);
                if (!emailResult.Success)
                {
                    // Rollback transaction if email fails
                    await transaction.RollbackAsync();
                    return new ApiResponse { Status = false, Message = $"Failed to send quotation email: {emailResult.ErrorMessage}" };
                }

                // 12. Commit transaction
                await transaction.CommitAsync();

                return new ApiResponse 
                { 
                    Status = true, 
                    Message = $"Bulk quotation sent successfully to {firstRequirement.Client?.Name}. " +
                             $"Quotation ID: {quotation.QuotationId}, " +
                             $"Requirements processed: {requirements.Count}, " +
                             $"Total cost: {totalCost:C}"
                };
            }
            catch (Exception ex)
            {
                // Rollback transaction on any error
                await transaction.RollbackAsync();
                return new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Failed to send bulk quotation: {ex.Message}" 
                };
            }
        });
    }

    // Helper method to validate bulk quotation request
    private (bool IsValid, string ErrorMessage) ValidateBulkQuotationRequest(BulkQuotationRequestDTO request)
    {
        // Check for duplicate requirement IDs
        var requirementIds = request.SelectedRequirements.Select(sr => sr.RequirementId).ToList();
        if (requirementIds.Count != requirementIds.Distinct().Count())
        {
            return (false, "Duplicate requirements found in selection.");
        }

        // Validate cost values
        if (request.SelectedRequirements.Any(sr => sr.QuotationCost <= 0))
        {
            return (false, "All requirement costs must be greater than zero.");
        }

        // Validate delivery dates
        var currentDate = DateTime.UtcNow.Date;
        if (request.SelectedRequirements.Any(sr => sr.DeliveryDate.Date < currentDate))
        {
            return (false, "Delivery dates cannot be in the past.");
        }

        return (true, string.Empty);
    }

    // Helper method to generate PDF for bulk quotation
    private byte[] GenerateBulkQuotationPdf(
        List<Requirement> requirements, 
        BulkQuotationRequestDTO bulkRequest, 
        Requirement firstRequirement, 
        Quotation quotation)
    {
        try
        {
            // Create a comprehensive description for the bulk quotation
            var requirementDetails = string.Join(Environment.NewLine, 
                requirements.Select((r, index) => 
                {
                    var selectedReq = bulkRequest.SelectedRequirements.First(sr => sr.RequirementId == r.RequirementId);
                    return $"{index + 1}. {r.RequirementTitle} - {selectedReq.QuotationCost:C} (Est: {selectedReq.EstimatedDuration})";
                }));

            var comprehensiveDescription = $"BULK QUOTATION DETAILS:{Environment.NewLine}" +
                                         $"Total Requirements: {requirements.Count}{Environment.NewLine}" +
                                         $"Total Cost: {quotation.TotalCost:C}{Environment.NewLine}{Environment.NewLine}" +
                                         $"INDIVIDUAL REQUIREMENTS:{Environment.NewLine}" +
                                         $"{requirementDetails}{Environment.NewLine}{Environment.NewLine}" +
                                         $"Additional Notes: {bulkRequest.AdditionalNotes}";

            var firstSelectedReq = bulkRequest.SelectedRequirements.First();
            var pdfBytes = _pdfService.GenerateRequirementQuotation(
                firstRequirement,
                new RequirementQuotationRequestDTO 
                {
                    RequirementId = firstSelectedReq.RequirementId,
                    QuotationCost = quotation.TotalCost,
                    EstimatedDuration = $"Bulk delivery - see individual timelines",
                    Description = comprehensiveDescription,
                    DeliveryDate = bulkRequest.SelectedRequirements.Max(sr => sr.DeliveryDate) // Latest delivery date
                },
                firstRequirement.Client?.ContactNo ?? "N/A",
                firstRequirement.Client?.Email ?? "N/A",
                firstRequirement.Project?.ProjectName ?? "N/A"
            );

            return pdfBytes;
        }
        catch (Exception ex)
        {
            // If PDF generation fails, create a simple fallback
            throw new Exception($"PDF generation failed: {ex.Message}");
        }
    }

    // Helper method to send bulk quotation email
    private async Task<(bool Success, string ErrorMessage)> SendBulkQuotationEmail(
        List<Requirement> requirements, 
        BulkQuotationRequestDTO bulkRequest, 
        Requirement firstRequirement, 
        Quotation quotation, 
        byte[] pdfBytes)
    {
        try
        {
            // Prepare Email Body
            var emailBody = await _emailService.GetEmailTemplateAsync("BulkRequirementQuotation.html");
            if (string.IsNullOrEmpty(emailBody))
            {
                // Fallback to a simple email template if the bulk template doesn't exist
                emailBody = await _emailService.GetEmailTemplateAsync("RequirementQuotation.html");
            }

            // Create detailed requirement list for email
            var requirementTitles = string.Join(", ", requirements.Select(r => r.RequirementTitle));
            var requirementSummary = string.Join("<br/>", 
                requirements.Select((r, index) => 
                {
                    var selectedReq = bulkRequest.SelectedRequirements.First(sr => sr.RequirementId == r.RequirementId);
                    return $"{index + 1}. {r.RequirementTitle} - {selectedReq.QuotationCost:C}";
                }));

            // Replace email template placeholders
            emailBody = emailBody.Replace("{{ClientAdminName}}", firstRequirement.Client?.Name ?? "N/A")
                                 .Replace("{{RequirementTitle}}", requirementTitles)
                                 .Replace("{{RequirementSummary}}", requirementSummary)
                                 .Replace("{{ProjectName}}", firstRequirement.Project?.ProjectName ?? "N/A")
                                 .Replace("{{ClientContact}}", firstRequirement.Client?.ContactNo ?? "N/A")
                                 .Replace("{{ClientEmail}}", firstRequirement.Client?.Email ?? "N/A")
                                 .Replace("{{TotalCost}}", quotation.TotalCost.ToString("C"))
                                 .Replace("{{RequirementCount}}", requirements.Count.ToString())
                                 .Replace("{{QuotationId}}", quotation.QuotationId.ToString())
                                 .Replace("{{AdditionalNotes}}", bulkRequest.AdditionalNotes ?? "None");

            // Send Email with PDF Attachment
            await _emailService.SendEmailWithAttachmentAsync(
                firstRequirement.Client?.Email ?? "",
                $"[Bulk Quotation #{quotation.QuotationId}] {firstRequirement.Project?.ProjectName} - G3NEXUS",
                emailBody,
                pdfBytes,
                $"Bulk_Quotation_{quotation.QuotationId}_{DateTime.Now:yyyyMMdd}.pdf",
                isHtml: true
            );

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}