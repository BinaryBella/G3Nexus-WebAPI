using System.ComponentModel.DataAnnotations;

namespace G3NexusBackend.Data.DTO;

public class ResetPasswordVerificationDTO
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    public string VerificationCode { get; set; }
}