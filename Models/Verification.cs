using System.ComponentModel.DataAnnotations;

namespace G3NexusBackend.Models;

public class Verification
{
    public int VId { get; set; }
    public string VerificationCode { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    public DateTime ExpiryDate { get; set; }
}