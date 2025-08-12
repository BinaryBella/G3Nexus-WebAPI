namespace G3NexusBackend.Data.DTO
{
    public class BulkQuotationRequestDTO
    {
        public List<RequirementQuotationItemDTO> SelectedRequirements { get; set; } = new();
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public string AdditionalNotes { get; set; } = string.Empty;
    }

    public class RequirementQuotationItemDTO
    {
        public int RequirementId { get; set; }
        public decimal QuotationCost { get; set; }
        public string EstimatedDuration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
    }
}
