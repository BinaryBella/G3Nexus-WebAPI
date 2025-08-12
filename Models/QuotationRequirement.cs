namespace G3NexusBackend.Models
{
    public class QuotationRequirement
    {
        public int QuotationRequirementId { get; set; }
        public int QuotationId { get; set; }
        public int RequirementId { get; set; }
        public decimal RequirementCost { get; set; }

        public Quotation? Quotation { get; set; }
        public Requirement? Requirement { get; set; }
    }
}