using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

public class ProjectService : IProjectService
{
    private readonly G3NexusDbContext _context;
    private readonly IPdfGeneratorService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IClientService _clientService;

    public ProjectService(G3NexusDbContext context, IPdfGeneratorService pdfService, IEmailService emailService, IClientService clientService)
    {
        _context = context;
        _pdfService = pdfService;
        _emailService = emailService;
        _clientService = clientService;
    }

    public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
    {
        return await _context.Projects
            .Where(p => p.IsActive)
            .Select(p => new ProjectDTO
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectType = p.ProjectType,
                ProjectSize = p.ProjectSize,
                CreationDate = p.CreationDate,
                ProjectDescription = p.ProjectDescription,
                EstimatedBudget = p.EstimatedBudget,
                ActualStartDate = p.ActualStartDate,
                ActualEndDate = p.ActualEndDate,
                TotalBudget = p.TotalBudget,
                PaymentType = p.PaymentType,
                PaymentStatus = p.PaymentStatus,
                Status = p.Status,
                IsActive = p.IsActive,
                CompanyId = p.CompanyId 
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectDTO>> GetProjectsByClientIdAsync(string email)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == email && c.IsActive);
        if (client == null)
        {
            return Enumerable.Empty<ProjectDTO>();
        }

        return await _context.Projects
            .Where(p => p.IsActive && p.CompanyId == client.CompanyId)
            .Select(p => new ProjectDTO
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectType = p.ProjectType,
                ProjectSize = p.ProjectSize,
                CreationDate = p.CreationDate,
                ProjectDescription = p.ProjectDescription,
                EstimatedBudget = p.EstimatedBudget,
                ActualStartDate = p.ActualStartDate,
                ActualEndDate = p.ActualEndDate,
                TotalBudget = p.TotalBudget,
                PaymentType = p.PaymentType,
                PaymentStatus = p.PaymentStatus,
                Status = p.Status,
                IsActive = p.IsActive,
                CompanyId = p.CompanyId
            })
            .ToListAsync();
    }

    public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project is not { IsActive: true })
        {
            return null;
        }

        return new ProjectDTO
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectType = project.ProjectType,
            ProjectSize = project.ProjectSize,
            CreationDate = project.CreationDate,
            ProjectDescription = project.ProjectDescription,
            EstimatedBudget = project.EstimatedBudget,
            ActualStartDate = project.ActualStartDate,
            ActualEndDate = project.ActualEndDate,
            TotalBudget = project.TotalBudget,
            PaymentType = project.PaymentType,
            PaymentStatus = project.PaymentStatus,
            Status = project.Status,
            IsActive = project.IsActive,
            CompanyId = project.CompanyId
        };
    }

    private async Task<ProjectResponseDTO> MapToResponseDtoAsync(ProjectDTO projectDto, int projectId)
    {
        // Get quotation cost from database
        var quotationCost = await _context.QuotationCosts
            .Where(qc => qc.ProjectId == projectId)
            .Select(qc => new QuotationCostDTO
            {
                AdvancePayment = qc.AdvancePayment,
                DevelopmentCost = qc.DevelopmentCost,
                HostingAndDomain = qc.HostingAndDomain,
                SSLCertificate = qc.SSLCertificate,
                ServerCost = qc.ServerCost,
                DeploymentCost = qc.DeploymentCost
            })
            .FirstOrDefaultAsync();

        // Get terms and conditions from database
        var termsConditions = await _context.ProjectTermsConditions
            .Where(ptc => ptc.ProjectId == projectId)
            .Select(ptc => new TermsConditionSelectionDTO
            {
                TCId = ptc.TCId,
                IsChecked = true // If it exists in the table, it's checked
            })
            .ToListAsync();

        return new ProjectResponseDTO
        {
            ProjectId = projectDto.ProjectId,
            ProjectName = projectDto.ProjectName,
            ProjectType = projectDto.ProjectType,
            ProjectSize = projectDto.ProjectSize,
            CreationDate = projectDto.CreationDate,
            ProjectDescription = projectDto.ProjectDescription,
            EstimatedBudget = projectDto.EstimatedBudget,
            ActualStartDate = projectDto.ActualStartDate,
            ActualEndDate = projectDto.ActualEndDate,
            TotalBudget = projectDto.TotalBudget,
            PaymentType = projectDto.PaymentType,
            PaymentStatus = projectDto.PaymentStatus,
            Status = projectDto.Status,
            IsActive = projectDto.IsActive,
            CompanyId = projectDto.CompanyId,
            QuotationCost = quotationCost,
            TermsConditions = termsConditions
        };
    }

    public async Task<ProjectDTO> CreateProjectAsync(AddProjectRequestDto projectRequestDto)
    {
        // 1. Validate Client Admin for the Company
        var client = await _clientService.GetClientAdminByCompanyIdAsync(projectRequestDto.CompanyId);

        if (client == null)
            throw new Exception("No active Client Admin found for the company.");

        // 2. Create Project Entity
        var project = new Project
        {
            ProjectName = projectRequestDto.ProjectName,
            ProjectType = projectRequestDto.ProjectType,
            ProjectSize = projectRequestDto.ProjectSize,
            CreationDate = projectRequestDto.CreationDate,
            ProjectDescription = projectRequestDto.ProjectDescription,
            EstimatedBudget = projectRequestDto.EstimatedBudget,
            ActualStartDate = projectRequestDto.ActualStartDate,
            ActualEndDate = projectRequestDto.ActualEndDate,
            TotalBudget = projectRequestDto.TotalBudget,
            PaymentType = projectRequestDto.PaymentType,
            PaymentStatus = projectRequestDto.PaymentStatus,
            Status = projectRequestDto.Status,
            IsActive = projectRequestDto.IsActive,
            CompanyId = projectRequestDto.CompanyId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(); // Generate ProjectId

        // 3. Save Quotation Cost
        var quotationCost = new QuotationCost
        {
            ProjectId = project.ProjectId,
            AdvancePayment = projectRequestDto.QuotationCost.AdvancePayment,
            DevelopmentCost = projectRequestDto.QuotationCost.DevelopmentCost,
            HostingAndDomain = projectRequestDto.QuotationCost.HostingAndDomain,
            SSLCertificate = projectRequestDto.QuotationCost.SSLCertificate,
            ServerCost = projectRequestDto.QuotationCost.ServerCost,
            DeploymentCost = projectRequestDto.QuotationCost.DeploymentCost
        };

        _context.QuotationCosts.Add(quotationCost);

        // 4. Save Selected Terms and Conditions
        var selectedTerms = projectRequestDto.TermsConditions
            .Where(tc => tc.IsChecked)
            .Select(tc => new ProjectTermsConditions
            {
                ProjectId = project.ProjectId,
                TCId = tc.TCId
            });

        _context.ProjectTermsConditions.AddRange(selectedTerms);

        await _context.SaveChangesAsync();

        // 5. Prepare DTO for PDF generation
        var projectDto = new ProjectDTO
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectType = project.ProjectType,
            ProjectSize = project.ProjectSize,
            CreationDate = project.CreationDate,
            ProjectDescription = project.ProjectDescription,
            EstimatedBudget = project.EstimatedBudget,
            ActualStartDate = project.ActualStartDate,
            ActualEndDate = project.ActualEndDate,
            TotalBudget = project.TotalBudget,
            PaymentType = project.PaymentType,
            PaymentStatus = project.PaymentStatus,
            Status = project.Status,
            IsActive = project.IsActive,
            CompanyId = project.CompanyId
        };

        // 6. Create ProjectResponseDTO with quotation details
        var projectResponseDto = new ProjectResponseDTO
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectType = project.ProjectType,
            ProjectSize = project.ProjectSize,
            CreationDate = project.CreationDate,
            ProjectDescription = project.ProjectDescription,
            EstimatedBudget = project.EstimatedBudget,
            ActualStartDate = project.ActualStartDate,
            ActualEndDate = project.ActualEndDate,
            TotalBudget = project.TotalBudget,
            PaymentType = project.PaymentType,
            PaymentStatus = project.PaymentStatus,
            Status = project.Status,
            IsActive = project.IsActive,
            CompanyId = project.CompanyId,
            QuotationCost = projectRequestDto.QuotationCost,
            TermsConditions = projectRequestDto.TermsConditions.Where(tc => tc.IsChecked).ToList()
        };

        
        // 7. Generate PDF
        var selectedTermsContent = selectedTerms
            .Select(tc => _context.TermsConditions.FirstOrDefault(t => t.TCId == tc.TCId)?.Content)
            .Where(content => content != null)
            .ToList();
        var pdfBytes = _pdfService.GenerateProjectQuotation(
            projectResponseDto,
            client.Name,
            client.ContactNo,
            client.Email,
            selectedTermsContent
        );

        // 8. Prepare Email Body
        var emailBody = await _emailService.GetEmailTemplateAsync("ProjectQuotation.html");
        emailBody = emailBody.Replace("{{ClientAdminName}}", client.Name)
                             .Replace("{{ProjectName}}", project.ProjectName)
                             .Replace("{{QuotationId}}", project.ProjectId.ToString())
                             .Replace("{{QuotationDate}}", DateTime.Now.ToString("dd/MM/yyyy"))
                             .Replace("{{ClientContact}}", client.ContactNo)
                             .Replace("{{ClientEmail}}", client.Email)
                             .Replace("{{ProjectDescription}}", project.ProjectDescription)
                             .Replace("{{StartDate}}", project.ActualStartDate?.ToShortDateString() ?? "N/A")
                             .Replace("{{EndDate}}", project.ActualEndDate?.ToShortDateString() ?? "N/A")
                             .Replace("{{TotalCost}}", project.TotalBudget.ToString("N2"));

        // 9. Send Email with PDF Attachment
        await _emailService.SendEmailWithAttachmentAsync(
            client.Email,
            $"[Quotation] {project.ProjectName} - G3NEXUS",
            emailBody,
            pdfBytes,
            "Project_Quotation.pdf",
            isHtml: true
        );

        return projectDto;
    }

    public async Task<ProjectDTO?> UpdateProjectAsync(int projectId, ProjectDTO projectDto)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project is not { IsActive: true })
        {
            return null;
        }

        project.ProjectName = projectDto.ProjectName;
        project.ProjectType = projectDto.ProjectType;
        project.ProjectSize = projectDto.ProjectSize;
        project.CreationDate = projectDto.CreationDate;
        project.ProjectDescription = projectDto.ProjectDescription;
        project.EstimatedBudget = projectDto.EstimatedBudget;
        project.ActualStartDate = projectDto.ActualStartDate;
        project.ActualEndDate = projectDto.ActualEndDate;
        project.TotalBudget = projectDto.TotalBudget;
        project.PaymentType = projectDto.PaymentType;
        project.PaymentStatus = projectDto.PaymentStatus;
        project.Status = projectDto.Status;
        project.CompanyId = projectDto.CompanyId;

        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return projectDto;
    }

    public async Task<ApiResponse> DeActivateProjectAsync(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null || !project.IsActive)
        {
            return new ApiResponse { Status = false, Message = "Project not found or already inactive." };
        }

        project.IsActive = false;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Project successfully deactivated." };
    }

    // Helper method to get project with full details for PDF generation
    public async Task<ProjectResponseDTO?> GetProjectWithDetailsAsync(int projectId)
    {
        var projectDto = await GetProjectByIdAsync(projectId);
        if (projectDto == null) return null;

        return await MapToResponseDtoAsync(projectDto, projectId);
    }
}