namespace G3NexusBackend.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public int ProjectId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDescription { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Attachment { get; set; }
    }
}