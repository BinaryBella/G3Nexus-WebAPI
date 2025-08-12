namespace G3NexusBackend.Data.DTO
{
    public class BugQuotationRequestDTO
    {
        public int BugId { get; set; }
        public decimal QuotationCost { get; set; }
        public string EstimatedDuration { get; set; } = string.Empty; // e.g., "5 days", "2 weeks"
        public string Description { get; set; } = string.Empty; // Additional description for the quotation
        public DateTime DeliveryDate { get; set; }
    }
}
