namespace G3NexusBackend.Models
{
    public class Payment
    {
        public int PaymentId { get; set; } // Primary Key
        public int ProjectId { get; set; }  // Foreign Key for Project
        public Project Project { get; set; }  // Navigation Property for the related Project
        public decimal PaymentAmount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDescription { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Attachment { get; set; }
        public bool IsActive { get; set; }
    }
}