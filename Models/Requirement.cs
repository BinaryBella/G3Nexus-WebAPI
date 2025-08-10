using G3NexusBackend.Models;

public class Requirement
{
    public int RequirementId { get; set; }
    public string? RequirementTitle { get; set; }
    public string? Priority { get; set; }
    public string? RequirementDescription { get; set; }
    public string? Attachment { get; set; }
    public bool IsActive { get; set; }
    public bool IsNew { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Foreign Keys
    public int ClientId { get; set; }
    public int ProjectId { get; set; }

    // Quotation Association
    public int? QuotationId { get; set; }
    public bool IsQuoted { get; set; } = false;

    // Navigation Properties
    public Client? Client { get; set; }
    public Project? Project { get; set; }
    public Quotation? Quotation { get; set; }
}