namespace G3NexusBackend.Data.DTO;

public class PaymentDTO
{
    public int PaymentId { get; set; }
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public decimal PaymentAmount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentDescription { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Attachment { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
}