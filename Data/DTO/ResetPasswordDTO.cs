namespace G3NexusBackend.DTOs
{
    public class ResetPasswordDTO
    {
        public string EmailAddress { get; set; }
        public string NewPassword { get; set; }
        public string Code { get; set; }
    }
}