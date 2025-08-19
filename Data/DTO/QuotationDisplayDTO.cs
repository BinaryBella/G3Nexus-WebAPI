using System;

namespace G3NexusBackend.Data.DTO
{
    public class QuotationDisplayDTO
    {
        public int QuotationId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        
        // Additional fields for modal popup display
        public List<QuotationItemDTO> Items { get; set; } = new List<QuotationItemDTO>();
        
        // Additional fields for better admin display
        public string FormattedCreatedDate => CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
        public string FormattedTotalCost => $"${TotalCost:N2}";
    }

    public class QuotationItemDTO
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty; // "Requirement" or "Bug"
        public string Title { get; set; } = string.Empty; // RequirementTitle or BugTitle
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // For requirements: requirement category, For bugs: bug category
        public string Priority { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string FormattedCost => $"${Cost:N2}";
        public string? Attachment { get; set; } // For requirements that have attachments
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string FormattedCreatedAt => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class QuotationDetailDTO : QuotationDisplayDTO
    {
        public List<QuotationRequirementDetailDTO> Requirements { get; set; } = new List<QuotationRequirementDetailDTO>();
        public List<QuotationBugDetailDTO> Bugs { get; set; } = new List<QuotationBugDetailDTO>();
        public QuotationCostDetailDTO? QuotationCost { get; set; }
    }

    public class QuotationRequirementDetailDTO
    {
        public int RequirementId { get; set; }
        public string RequirementDescription { get; set; } = string.Empty;
        public string RequirementType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
    }

    public class QuotationBugDetailDTO
    {
        public int BugId { get; set; }
        public string BugDescription { get; set; } = string.Empty;
        public string BugType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
    }

    public class QuotationCostDetailDTO
    {
        public decimal AdvancePayment { get; set; }
        public decimal DevelopmentCost { get; set; }
        public decimal HostingAndDomain { get; set; }
        public decimal SSLCertificate { get; set; }
        public decimal DeploymentCost { get; set; }
        public decimal TotalProjectCost => AdvancePayment + DevelopmentCost + HostingAndDomain + SSLCertificate + DeploymentCost;
    }

    public class QuotationSummaryDTO
    {
        public int TotalQuotations { get; set; }
        public int RequirementQuotations { get; set; }
        public int BugQuotations { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AverageValue { get; set; }
        public DateTime? LatestQuotationDate { get; set; }
        public DateTime? OldestQuotationDate { get; set; }
    }
}
