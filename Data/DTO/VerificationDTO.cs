namespace G3NexusBackend.DTOs
{
    public class VerificationDTO
    {
        public int VId { get; set; }
        public int UserId { get; set; }
        public string VerificationCode { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}