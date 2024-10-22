public class ProjectEmployeeClient
{
    public int ProjectId { get; set; } // Foreign Key
    public int EmployeeId { get; set; } // Foreign Key
    public int ClientId { get; set; } // Foreign Key
    public Project Project { get; set; } // Navigation property
    public Employee Employee { get; set; } // Navigation property
    public Client Client { get; set; } // Navigation property
}