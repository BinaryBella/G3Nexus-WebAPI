namespace G3NexusBackend.Data.DTO
{
    public class BulkBugQuotationRequestDTO
    {
        public List<BugQuotationItemDTO> SelectedBugs { get; set; } = new();
        public int ClientId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string AdditionalNotes { get; set; } = string.Empty;
    }

    public class BugQuotationItemDTO
    {
        public int BugId { get; set; }
        public decimal QuotationCost { get; set; }
        public string EstimatedDuration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
    }
}
