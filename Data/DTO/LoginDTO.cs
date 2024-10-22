using System.ComponentModel.DataAnnotations;

namespace G3NexusBackend.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}