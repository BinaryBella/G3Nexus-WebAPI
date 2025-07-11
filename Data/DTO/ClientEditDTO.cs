using System.ComponentModel.DataAnnotations;

namespace G3NexusBackend.Data.DTO
{
    public class ClientEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }
}