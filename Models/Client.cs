public class Client
{
    public int Id { get; set; } // Primary Key
    public string Name { get; set; }
    public string ContactNo { get; set; } 
    public string Email { get; set; } 
    public string Address { get; set; }
    public string Password { get; set; } 
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Requirement> Requirements { get; set; } // Navigation property for related requirements
    public ICollection<Bug> Bugs { get; set; } // Navigation property for related bugs
    // public int CompanyId { get; set; }
    // public Company Company { get; set; }
}