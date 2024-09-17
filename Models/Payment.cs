namespace G3NexusBackend.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }  
        public decimal PaymentAmount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDescription { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Attachment { get; set; }
    }
}
