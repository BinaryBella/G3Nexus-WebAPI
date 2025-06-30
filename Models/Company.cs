namespace G3NexusBackend.Models;

public class Company
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
    
    // public ICollection<Client> Clients { get; set; } // Navigation property for related clients
}