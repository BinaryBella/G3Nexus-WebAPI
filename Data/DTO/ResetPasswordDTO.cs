namespace G3NexusBackend.Data.DTO
{
    public class ResetPasswordDTO
    {
        public string EmailAddress { get; set; }
        public string NewPassword { get; set; }
        public string VerificationCode { get; set; }
    }
}