using G3NexusBackend.Models;

public class Employee
{
    public int EmployeeId { get; set; } // Primary Key
    public string Name { get; set; } 
    public string ContactNo { get; set; } 
    public string Email { get; set; } 
    public string Address { get; set; }
    public string Password { get; set; } 
    public string Role { get; set; }
    public string ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    // Navigation property
    public ICollection<EmployeeProject> EmployeeProjects { get; set; }
}