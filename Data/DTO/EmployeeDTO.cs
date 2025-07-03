using System.ComponentModel.DataAnnotations;

namespace G3NexusBackend.Data.DTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; } 
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}