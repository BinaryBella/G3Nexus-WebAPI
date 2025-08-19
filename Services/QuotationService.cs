using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Data;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;

namespace G3NexusBackend.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly G3NexusDbContext _context;

        public QuotationService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuotationDisplayDTO>> GetAllQuotationsAsync()
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<QuotationDetailDTO?> GetQuotationByIdAsync(int quotationId)
        {
            var quotation = await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                    .ThenInclude(p => p.QuotationCost)
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(q => q.QuotationId == quotationId);

            if (quotation == null)
                return null;

            var result = new QuotationDetailDTO
            {
                QuotationId = quotation.QuotationId,
                ClientId = quotation.ClientId,
                ClientName = quotation.Client?.Name ?? "Unknown",
                ClientEmail = quotation.Client?.Email ?? "Unknown",
                ProjectId = quotation.ProjectId,
                ProjectName = quotation.Project?.ProjectName ?? "Unknown",
                ProjectDescription = quotation.Project?.ProjectDescription ?? "Unknown",
                EmployeeId = quotation.EmployeeId,
                EmployeeName = quotation.Employee?.Name ?? "Unknown",
                EmployeeEmail = quotation.Employee?.Email ?? "Unknown",
                CreatedDate = quotation.CreatedDate,
                Type = quotation.Type,
                TotalCost = quotation.TotalCost
            };

            // Get quotation costs if available
            if (quotation.Project?.QuotationCost != null)
            {
                result.QuotationCost = new QuotationCostDetailDTO
                {
                    AdvancePayment = quotation.Project.QuotationCost.AdvancePayment,
                    DevelopmentCost = quotation.Project.QuotationCost.DevelopmentCost,
                    HostingAndDomain = quotation.Project.QuotationCost.HostingAndDomain,
                    SSLCertificate = quotation.Project.QuotationCost.SSLCertificate,
                    DeploymentCost = quotation.Project.QuotationCost.DeploymentCost
                };
            }

            // Get related requirements
            var quotationRequirements = await _context.QuotationRequirements!
                .Include(qr => qr.Requirement)
                .Where(qr => qr.QuotationId == quotationId)
                .Select(qr => new QuotationRequirementDetailDTO
                {
                    RequirementId = qr.RequirementId,
                    RequirementDescription = qr.Requirement != null ? qr.Requirement.RequirementDescription ?? "Unknown" : "Unknown",
                    RequirementType = qr.Requirement != null ? qr.Requirement.RequirementTitle ?? "Unknown" : "Unknown",
                    Priority = qr.Requirement != null ? qr.Requirement.Priority ?? "Unknown" : "Unknown",
                    EstimatedCost = qr.RequirementCost
                })
                .ToListAsync();

            result.Requirements = quotationRequirements;

            // Get related bugs
            var quotationBugs = await _context.QuotationBugs!
                .Include(qb => qb.Bug)
                .Where(qb => qb.QuotationId == quotationId)
                .Select(qb => new QuotationBugDetailDTO
                {
                    BugId = qb.BugId,
                    BugDescription = qb.Bug != null ? qb.Bug.BugDescription ?? "Unknown" : "Unknown",
                    BugType = qb.Bug != null ? qb.Bug.BugTitle ?? "Unknown" : "Unknown",
                    Priority = qb.Bug != null ? qb.Bug.Severity ?? "Unknown" : "Unknown",
                    EstimatedCost = qb.BugCost
                })
                .ToListAsync();

            result.Bugs = quotationBugs;

            return result;
        }

        public async Task<List<QuotationDisplayDTO>> GetQuotationsByClientIdAsync(int clientId)
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Where(q => q.ClientId == clientId)
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<QuotationDisplayDTO>> GetQuotationsByProjectIdAsync(int projectId)
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Where(q => q.ProjectId == projectId)
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<QuotationDisplayDTO>> GetQuotationsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Where(q => q.EmployeeId == employeeId)
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<QuotationDisplayDTO>> GetQuotationsByTypeAsync(string type)
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Where(q => q.Type.ToLower() == type.ToLower())
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<QuotationDisplayDTO>> GetQuotationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Quotations!
                .Include(q => q.Client)
                .Include(q => q.Project)
                .Include(q => q.Employee)
                .Where(q => q.CreatedDate >= startDate && q.CreatedDate <= endDate)
                .Select(q => new QuotationDisplayDTO
                {
                    QuotationId = q.QuotationId,
                    ClientId = q.ClientId,
                    ClientName = q.Client != null ? q.Client.Name : "Unknown",
                    ClientEmail = q.Client != null ? q.Client.Email : "Unknown",
                    ProjectId = q.ProjectId,
                    ProjectName = q.Project != null ? q.Project.ProjectName : "Unknown",
                    ProjectDescription = q.Project != null ? q.Project.ProjectDescription : "Unknown",
                    EmployeeId = q.EmployeeId,
                    EmployeeName = q.Employee != null ? q.Employee.Name : "Unknown",
                    EmployeeEmail = q.Employee != null ? q.Employee.Email : "Unknown",
                    CreatedDate = q.CreatedDate,
                    Type = q.Type,
                    TotalCost = q.TotalCost
                })
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<QuotationSummaryDTO> GetQuotationSummaryAsync()
        {
            var quotations = await _context.Quotations!.ToListAsync();

            if (!quotations.Any())
            {
                return new QuotationSummaryDTO
                {
                    TotalQuotations = 0,
                    RequirementQuotations = 0,
                    BugQuotations = 0,
                    TotalValue = 0,
                    AverageValue = 0,
                    LatestQuotationDate = null,
                    OldestQuotationDate = null
                };
            }

            return new QuotationSummaryDTO
            {
                TotalQuotations = quotations.Count,
                RequirementQuotations = quotations.Count(q => q.Type.ToLower() == "requirement"),
                BugQuotations = quotations.Count(q => q.Type.ToLower() == "bug"),
                TotalValue = quotations.Sum(q => q.TotalCost),
                AverageValue = quotations.Average(q => q.TotalCost),
                LatestQuotationDate = quotations.Max(q => q.CreatedDate),
                OldestQuotationDate = quotations.Min(q => q.CreatedDate)
            };
        }
    }
}
