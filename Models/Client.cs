using G3NexusBackend.Models;

public class Client
{
    public int Id { get; set; } // Primary Key
    public string Name { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int CompanyId { get; set; } // Foreign Key to Company
    public Company Company { get; set; } // Navigation property for related company
    public ICollection<Requirement> Requirements { get; set; } // Navigation property for related requirements
    public ICollection<Bug> Bugs { get; set; } // Navigation property for related bugs
    public ICollection<Quotation> Quotations { get; set; } // Navigation property for related quotations
}