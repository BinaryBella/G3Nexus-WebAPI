namespace G3NexusBackend.Data.DTO;

public class AddProjectRequestDto
{
    // Project details
    public string ProjectName { get; set; }
    public string ProjectType { get; set; }
    public string ProjectSize { get; set; }
    public DateTime CreationDate { get; set; }
    public string ProjectDescription { get; set; }
    public decimal EstimatedBudget { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal TotalBudget { get; set; }
    public string PaymentType { get; set; }
    public string PaymentStatus { get; set; }
    public string Status { get; set; }
    public bool IsActive { get; set; }
    public int CompanyId { get; set; }

    // Quotation cost breakdown
    public QuotationCostDTO QuotationCost { get; set; }

    // Selected terms & conditions (by TCId and IsChecked)
    public List<TermsConditionSelectionDTO> TermsConditions { get; set; }
}
