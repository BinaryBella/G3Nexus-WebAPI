namespace G3NexusBackend.Data.DTO
{
    public class RequirementQuotationRequestDTO
    {
        public int RequirementId { get; set; }
        public int EmployeeId { get; set; } // ID of the employee creating the quotation
        public decimal QuotationCost { get; set; }
        public string EstimatedDuration { get; set; } // e.g., "5 days", "2 weeks"
        public string Description { get; set; } // Additional description for the quotation
        public DateTime DeliveryDate { get; set; }
    }
}
