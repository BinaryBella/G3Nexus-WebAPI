namespace G3NexusBackend.Models;

public class Payment
{
    public int PaymentId { get; set; } // Primary Key
    public int ProjectId { get; set; }  // Foreign Key for Project
    public Project Project { get; set; } = null!;  // Navigation Property for the related Project
    public int ClientId { get; set; }  // Foreign Key for Client
    public Client Client { get; set; } = null!;  // Navigation Property for the related Client
    public decimal PaymentAmount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentDescription { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Attachment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}