using G3NexusBackend.Models;

public class Project
{
    public int ProjectId { get; set; } // Primary Key
    public string ProjectName { get; set; }
    public string ProjectType { get; set; }
    public string ProjectSize { get; set; }
    public DateTime CreationDate { get; set; } 
    public string ProjectDescription { get; set; } 
    public decimal EstimatedBudget { get; set; } 
    public DateTime? ActualStartDate { get; set; } 
    public DateTime? ActualEndDate { get; set; } 
    public decimal TotalBudget { get; set; } 
    public string PaymentType { get; set; } 
    public string PaymentStatus { get; set; } 
    public string Status { get; set; }
    public bool IsActive { get; set; }

    public ICollection<ProjectEmployeeClient> ProjectEmployeeClients { get; set; } // Navigation property
    public ICollection<Requirement> Requirements { get; set; } // Navigation property for related requirements
    public ICollection<Bug> Bugs { get; set; } // Navigation property for related bugs
    public ICollection<Payment> Payments { get; set; } // Navigation property for related payments
}