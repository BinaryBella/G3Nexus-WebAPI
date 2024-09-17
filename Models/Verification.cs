namespace G3NexusBackend.Models
{
    public class Verification
    {
        public int VId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string VerificationCode { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
